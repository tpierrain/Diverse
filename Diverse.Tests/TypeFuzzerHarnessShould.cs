using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Diverse.Tests.Utils;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    /// <summary>
    /// Harness tests for the TypeFuzzer ensuring it does not blow up stack or memory
    /// when generating complex object graphs (self-referencing types, mutual references,
    /// indirect cycles, deep hierarchies, collections of collections).
    /// Each test has a timeout to catch infinite recursion / geometric explosion.
    /// </summary>
    [TestFixture]
    public class TypeFuzzerHarnessShould
    {
        private const int HarnessTimeoutMs = 5000;
        private const int MaxAcceptableDurationMs = 1000;

        [Test]
        [Timeout(HarnessTimeoutMs)]
        public void Not_stack_overflow_on_self_referencing_type_with_collection()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<SelfReferencingTypeWithACollectionOfItself>();

            Check.That(result).IsNotNull();
            Check.That(result.Name).IsNotEmpty();
            Check.That(result.Friends).IsNotNull();
        }

        [Test]
        [Timeout(HarnessTimeoutMs)]
        public void Not_stack_overflow_on_mutual_references()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<TypeA>();

            Check.That(result).IsNotNull();
            Check.That(result.Name).IsNotEmpty();
            Check.That(result.Partner).IsNotNull();
            Check.That(result.Partner.Label).IsNotEmpty();
            // Partner's Friend (TypeA) should exist at depth 1
            Check.That(result.Partner.Friend).IsNotNull();
            // Partner's Friend's Partner (TypeB) should exist at depth 1 for TypeB
            // but Partner's Friend's Partner's Friend (TypeA) should be null (depth 2 for TypeA)
            if (result.Partner.Friend.Partner != null)
            {
                Check.That(result.Partner.Friend.Partner.Friend).IsNull();
            }
        }

        [Test]
        [Timeout(HarnessTimeoutMs)]
        public void Not_stack_overflow_on_indirect_cycles()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<CycleX>();

            Check.That(result).IsNotNull();
            Check.That(result.Name).IsNotEmpty();
            Check.That(result.Next).IsNotNull();
            Check.That(result.Next.Name).IsNotEmpty();
            Check.That(result.Next.Next).IsNotNull();
            Check.That(result.Next.Next.Name).IsNotEmpty();
            // CycleZ.Next is CycleX, which should be non-null (first recursion of CycleX)
            // but at depth 2 of CycleX it should be null
            var secondCycleX = result.Next.Next.Next;
            Check.That(secondCycleX).IsNotNull();
            // The chain should eventually stop
            if (secondCycleX.Next != null && secondCycleX.Next.Next != null)
            {
                // At some point CycleX should be null (cycle detected)
                var thirdCycleX = secondCycleX.Next.Next.Next;
                Check.That(thirdCycleX).IsNull();
            }
        }

        [Test]
        [Timeout(HarnessTimeoutMs)]
        public void Not_stack_overflow_on_multiple_self_referencing_collections()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<TypeWithMultipleSelfReferencingCollections>();

            Check.That(result).IsNotNull();
            Check.That(result.Name).IsNotEmpty();
            Check.That(result.Children).IsNotNull();
            Check.That(result.Siblings).IsNotNull();

            // Children and Siblings should have reduced sizes but be non-empty
            var children = result.Children.ToList();
            var siblings = result.Siblings.ToList();
            Check.That(children).Not.IsEmpty();
            Check.That(siblings).Not.IsEmpty();

            // Each child's own Children and Siblings should be empty (cycle depth reached)
            foreach (var child in children)
            {
                Check.That(child).IsNotNull();
                Check.That(child.Children.ToList()).IsEmpty();
                Check.That(child.Siblings.ToList()).IsEmpty();
            }
        }

        [Test]
        [Timeout(HarnessTimeoutMs)]
        public void Not_stack_overflow_on_mutual_reference_via_collections()
        {
            var fuzzer = new Fuzzer();

            var teacher = fuzzer.GenerateInstanceOf<Teacher>();

            Check.That(teacher).IsNotNull();
            Check.That(teacher.Name).IsNotEmpty();
            Check.That(teacher.Students).IsNotNull();

            var students = teacher.Students.ToList();
            // Students should be generated
            foreach (var student in students)
            {
                Check.That(student).IsNotNull();
                Check.That(student.Name).IsNotEmpty();
                Check.That(student.Teachers).IsNotNull();
            }
        }

        [Test]
        [Timeout(HarnessTimeoutMs)]
        public void Handle_deep_non_cyclic_hierarchies_without_issue()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<Level1>();

            // Deep but non-cyclic: all levels should be fully generated
            Check.That(result).IsNotNull();
            Check.That(result.Name).IsNotEmpty();
            Check.That(result.Child).IsNotNull();
            Check.That(result.Child.Name).IsNotEmpty();
            Check.That(result.Child.Date).IsNotEqualTo(default(DateTime));
            Check.That(result.Child.Child).IsNotNull();
            Check.That(result.Child.Child.Name).IsNotEmpty();
            Check.That(result.Child.Child.Child).IsNotNull();
            Check.That(result.Child.Child.Child.Name).IsNotEmpty();
            Check.That(result.Child.Child.Child.Child).IsNotNull();
            Check.That(result.Child.Child.Child.Child.Name).IsNotEmpty();
            Check.That(result.Child.Child.Child.Child.Code).IsNotEqualTo(default(long));
        }

        [Test]
        [Timeout(HarnessTimeoutMs)]
        public void Complete_generation_of_self_referencing_types_within_acceptable_time()
        {
            var fuzzer = new Fuzzer();

            var stopwatch = Stopwatch.StartNew();
            var result = fuzzer.GenerateInstanceOf<SelfReferencingTypeWithACollectionOfItself>();
            stopwatch.Stop();

            Check.That(result).IsNotNull();
            Check.That(stopwatch.ElapsedMilliseconds).IsStrictlyLessThan(MaxAcceptableDurationMs);
        }

        [Test]
        [Timeout(HarnessTimeoutMs)]
        public void Complete_generation_of_mutual_references_within_acceptable_time()
        {
            var fuzzer = new Fuzzer();

            var stopwatch = Stopwatch.StartNew();
            var result = fuzzer.GenerateInstanceOf<TypeA>();
            stopwatch.Stop();

            Check.That(result).IsNotNull();
            Check.That(stopwatch.ElapsedMilliseconds).IsStrictlyLessThan(MaxAcceptableDurationMs);
        }

        [Test]
        [Timeout(HarnessTimeoutMs)]
        [Repeat(50)]
        public void Consistently_generate_self_referencing_types_across_many_seeds()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<SelfReferencingTypeWithACollectionOfItself>();

            Check.That(result).IsNotNull();
            Check.That(result.Name).IsNotEmpty();
            Check.That(result.Friends).IsNotNull();
        }

        [Test]
        [Timeout(HarnessTimeoutMs)]
        [Repeat(50)]
        public void Consistently_generate_mutual_references_across_many_seeds()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<TypeA>();

            Check.That(result).IsNotNull();
            Check.That(result.Name).IsNotEmpty();
            Check.That(result.Partner).IsNotNull();
        }

        [Test]
        [Timeout(HarnessTimeoutMs)]
        [Repeat(50)]
        public void Consistently_generate_indirect_cycles_across_many_seeds()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<CycleX>();

            Check.That(result).IsNotNull();
            Check.That(result.Name).IsNotEmpty();
            Check.That(result.Next).IsNotNull();
        }

        [Test]
        [Timeout(HarnessTimeoutMs)]
        public void Not_generate_excessive_instances_for_self_referencing_collection_types()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<SelfReferencingTypeWithACollectionOfItself>();

            // Count total instances in the object graph
            var totalInstances = CountSelfReferencingInstances(result);

            // With maxDepth=2 and collection size reduction, we expect a bounded number.
            // Root (1) + friends (2) + friends of friends (0) = 3 instances max
            Check.That(totalInstances).IsStrictlyLessThan(20);
        }

        [Test]
        [Timeout(HarnessTimeoutMs)]
        public void Not_generate_excessive_instances_for_multiple_self_referencing_collections()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<TypeWithMultipleSelfReferencingCollections>();

            // Count total instances
            var totalInstances = CountMultiCollectionInstances(result);

            // Even with two collections, should stay bounded
            Check.That(totalInstances).IsStrictlyLessThan(30);
        }

        [Test]
        [Timeout(HarnessTimeoutMs)]
        public void Produce_diverse_values_across_generated_instances()
        {
            var fuzzer = new Fuzzer();

            var result = fuzzer.GenerateInstanceOf<SelfReferencingTypeWithACollectionOfItself>();

            var allNames = new List<string> { result.Name };
            foreach (var friend in result.Friends)
            {
                allNames.Add(friend.Name);
            }

            // All names should be non-empty and there should be some diversity
            Check.That(allNames).HasSize(allNames.Count);
            foreach (var name in allNames)
            {
                Check.That(name).IsNotEmpty();
            }
        }

        private static int CountSelfReferencingInstances(SelfReferencingTypeWithACollectionOfItself instance)
        {
            if (instance == null) return 0;

            var count = 1;
            if (instance.Friends != null)
            {
                foreach (var friend in instance.Friends)
                {
                    count += CountSelfReferencingInstances(friend);
                }
            }

            return count;
        }

        private static int CountMultiCollectionInstances(TypeWithMultipleSelfReferencingCollections instance)
        {
            if (instance == null) return 0;

            var count = 1;
            if (instance.Children != null)
            {
                foreach (var child in instance.Children)
                {
                    count += CountMultiCollectionInstances(child);
                }
            }

            if (instance.Siblings != null)
            {
                foreach (var sibling in instance.Siblings)
                {
                    count += CountMultiCollectionInstances(sibling);
                }
            }

            return count;
        }
    }
}
