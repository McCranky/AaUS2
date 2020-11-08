using System;
using System.Collections.Generic;
using System.Linq;
using Structures.Trees.KDTree;

namespace StructureTestingApp
{
    public class KDTTester
    {
        private int _shuffleCounter = 0;
        private int TreeCount => _tree?.Count ?? 0;
        private int ListCount => _insertedKeys?.Count ?? 0;
        private int Input { get; set; }
        private bool Exit { get; set; }
        private Random Generator { get; set; }
        private int _keyCount;
        private LinkedList<IComparable[]> _insertedKeys;
        private KDTree<IComparable, string> _tree;
        public void Start()
        {
            Console.WriteLine("---  K-D Tree Tester  ---");
            
            Console.Write("Number of keys (default 2): ");
            var inp = Console.ReadLine();
            _keyCount = string.IsNullOrWhiteSpace(inp) ? 2 : int.Parse(inp);
            Console.WriteLine($"Key count = {_keyCount}");
            
            Console.Write("Seed (default is random): ");
            inp = Console.ReadLine();
            var seed = string.IsNullOrWhiteSpace(inp) ? Guid.NewGuid().GetHashCode() : int.Parse(inp);
            Console.WriteLine($"Seed = {seed}");
            Generator = new Random(seed);
            Console.WriteLine("Tree and support check list created.");
            _tree = new KDTree<IComparable, string>(_keyCount);
            _insertedKeys = new LinkedList<IComparable[]>();
            do
            {
                Console.WriteLine();
                PrintStructures();
                Console.WriteLine();
                PrintMenu();
                GetInput();
                HandleInput();
            } while (!Exit);
        }

        private void DoInsert(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var keys = GenerateKeys(_keyCount);
                _insertedKeys.AddFirst(keys.ToArray());
                _tree.Add(keys, i.ToString());
            }
            // kvôli náhodnosti mazania a hladanie prehádžeme zoznam
            if (_shuffleCounter++ % 5000 == 0 && _insertedKeys.Count > 1)
            {
                _insertedKeys = new LinkedList<IComparable[]>(_insertedKeys.OrderBy(o => Generator.Next()));
            }
        }
        
        private void DoDelete(int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (_insertedKeys.First == null) continue;
                var toRemove = _insertedKeys.First.Value;
                _tree.Remove(toRemove, Guid.Empty);
                _insertedKeys.Remove(toRemove);
            }
        }

        private bool DoFind(IComparable[] keys = null)
        {
            if (keys != null)
            {
                return _tree.TryFindKdtNodes(keys, out var result);
            }
            else
            {
                var toFind = _insertedKeys.ElementAt(0);//Generator.Next(_insertedKeys.Count - 1)
                return _tree.TryFindKdtNodes(toFind, out var result);
            }
            
        }
        
        private void DoRandomOperations(int opCount)
        {
            var failures = 0;
            for (var i = 0; i < opCount; i++)
            {
                var probability = Generator.NextDouble();
                if (probability < 0.5) // insert
                {
                    try
                    {
                        DoInsert(1);
                    }
                    catch (Exception e)
                    {
                        ++failures;
                    }
                }
                else if (probability < 0.75) // find
                {
                    if (ListCount <= 0) continue;
                    if (!DoFind())
                        ++failures;
                }
                else // delete
                {
                    try
                    {
                        DoDelete(1);
                    }
                    catch (Exception e)
                    {
                        ++failures;
                    }
                }
            }
            Console.WriteLine($"Failures: {failures}");
        }
        
        private void HandleInput()
        {
            switch (Input)
            {
                case 4:
                    Exit = true;
                    break;
                case 3: // random operations
                    Console.Write("Number of operations: ");
                    var operationsCount = int.Parse(Console.ReadLine()!);
                    DoRandomOperations(operationsCount);
                    break;
                case 2: // Delete
                    Console.Write("Number of records to delete: ");
                    var deleteCount = int.Parse(Console.ReadLine() ?? $"{TreeCount}") % (TreeCount + 1);
                    DoDelete(deleteCount);
                    break;
                case 1: // Find
                    Console.WriteLine($"Specify {_keyCount} keys.");
                    var userKeys = new List<IComparable>(_keyCount);
                    for (var i = 0; i < _keyCount; i++)
                    {
                        Console.Write($"Key {i + 1}: ");
                        userKeys.Add(int.Parse(Console.ReadLine()!));
                    }

                    Console.WriteLine(DoFind(userKeys.ToArray())
                        ? "Node was funded."
                        : "Node with given keys doesn't exist.");
                    break;
                case 0: // Insert
                    Console.Write("Number of records (default 1): ");
                    var count = int.Parse(Console.ReadLine() ?? "1");
                    DoInsert(count);
                    break;
                default:
                    Exit = true;
                    break;
            }
        }

        private void GetInput()
        {
            Console.Write("TesterInput:~ $ ");
            Input = int.Parse(Console.ReadLine() ?? "3");
        }

        private void PrintStructures()
        {
            Console.WriteLine($"Tree count: {TreeCount}");
            Console.WriteLine($"List count: {ListCount}");
        }

        private void PrintMenu()
        {
            Console.WriteLine("[0] Insert");
            Console.WriteLine("[1] Find");
            Console.WriteLine("[2] Delete");
            Console.WriteLine("[3] Random Operations");
            Console.WriteLine("[4] Exit");
        }

        private IComparable[] GenerateKeys(int count)
        {
            var keys = new List<IComparable>(_keyCount);
            for (var j = 0; j < count; j++)
            {
                keys.Add(Generator.Next());
            }

            return keys.ToArray();
        }
    }
}