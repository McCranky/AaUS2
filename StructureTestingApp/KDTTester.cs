using System;
using System.Collections.Generic;
using System.Linq;
using Structures.Trees.KDTree;

namespace StructureTestingApp
{
    public class KDTTester
    {
        public int TreeCount => _tree?.Count ?? 0;
        public int ListCount => _insertedKeys?.Count ?? 0;
        public int Input { get; set; }
        public bool Exit { get; set; }
        public Random Generator { get; set; }
        private int _keyCount;
        private LinkedList<IComparable[]> _insertedKeys;
        private KDTree<IComparable, string> _tree;
        public void Start()
        {
            Console.WriteLine("---  K-D Tree Tester  ---");
            
            Console.Write("Number of keys (defaut 2): ");
            var inp = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(inp))
            {
                _keyCount = 2;
            }
            else
            {
                _keyCount = int.Parse(inp);
            }
            Console.WriteLine($"Key count = {_keyCount}");
            
            Console.Write("Seed (default is random): ");
            inp = Console.ReadLine();
            int seed;
            if (string.IsNullOrWhiteSpace(inp))
            {
                seed = Guid.NewGuid().GetHashCode();
            }
            else
            {
                seed = int.Parse(inp);
            }
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

        private void HandleInput()
        {
            switch (Input)
            {
                case 3:
                    Exit = true;
                    break;
                case 2: // Delete
                    Console.Write("Manual [y/n]: ");
                    var manual = Console.ReadKey().KeyChar == 'y';
                    Console.WriteLine();
                    if (manual)
                    {
                        Console.WriteLine($"Specify {_keyCount} keys for deleting node.");
                        var deleteKeys = new List<IComparable>(_keyCount);
                        for (int i = 0; i < _keyCount; i++)
                        {
                            Console.Write($"Key {i + 1}: ");
                            deleteKeys.Add(int.Parse(Console.ReadLine()!));
                        }
                        _tree.Remove(deleteKeys, Guid.Empty);
                        IComparable[] listItem = null;
                        foreach (var keyPair in _insertedKeys)
                        {
                            if (keyPair.SequenceEqual(deleteKeys))
                            {
                                listItem = keyPair;
                            }
                        }

                        if (listItem != null)
                        {
                            _insertedKeys.Remove(listItem);
                        }
                    }
                    else
                    {
                        Console.Write("Number of records to delete: ");
                        var deleteCount = int.Parse(Console.ReadLine() ?? $"{TreeCount}") % (TreeCount + 1);
                        // kvôli náhodnosti prehádžeme zoznam
                        _insertedKeys = new LinkedList<IComparable[]>(_insertedKeys.OrderBy(o => Generator.Next()));
                        for (int i = 0; i < deleteCount; i++)
                        {
                            var toRemove = _insertedKeys.First.Value;
                            _tree.Remove(toRemove, Guid.Empty);
                            _insertedKeys.Remove(toRemove);
                        }
                    }
                    break;
                case 1: // Find
                    Console.WriteLine($"Specify {_keyCount} keys.");
                    var userKeys = new List<IComparable>(_keyCount);
                    for (int i = 0; i < _keyCount; i++)
                    {
                        Console.Write($"Key {i + 1}: ");
                        userKeys.Add(int.Parse(Console.ReadLine()!));
                    }

                    if (_tree.TryFindKdtNodes(userKeys, out var vals))
                    {
                        Console.WriteLine($"Number of matches: {vals.Count}");
                        foreach (var val in vals)
                        {
                            Console.WriteLine($" - {val.Data}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Node with given keys doesn't exist.");
                    }
                    break;
                case 0: // Insert
                    Console.Write("Manual [y/n]: ");
                    var manuall = Console.ReadKey().KeyChar == 'y';
                    Console.WriteLine();
                    if (manuall)
                    {
                        Console.WriteLine($"Specify {_keyCount} keys for inserting node.");
                        var insertKeys = new List<IComparable>(_keyCount);
                        for (int i = 0; i < _keyCount; i++)
                        {
                            Console.Write($"Key {i + 1}: ");
                            insertKeys.Add(int.Parse(Console.ReadLine()!));
                        }

                        Console.Write("Data: ");
                        var data = Console.ReadLine();
                        _insertedKeys.AddLast(insertKeys.ToArray());
                        _tree.Add(insertKeys, data);
                    }
                    else
                    {
                        Console.Write("Number of records (default 1): ");
                        var count = int.Parse(Console.ReadLine() ?? "1");
                        for (int i = 0; i < count; i++)
                        {
                            var keys = new List<IComparable>(_keyCount);
                            for (int j = 0; j < _keyCount; j++)
                            {
                                keys.Add(Generator.Next());
                            }
                            _insertedKeys.AddLast(keys.ToArray());
                            _tree.Add(keys, i.ToString());
                        }
                    }
                    Console.WriteLine("Insertion completed.");
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
            Console.WriteLine("[3] Exit");
        }
        
        
    }
}