using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Student_Result_Processing
{
    class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> Marks { get; set; } = new List<int>();

        public double Cal_Avg()
        {
            if(Marks.Count == 0) return 0;
            double total = 0;

            foreach(var marks in Marks)
            {
                total += marks;
            }
            return total / Marks.Count;
        }

        public string GetGrade()
        {
            double avg = Cal_Avg();
            if (avg >= 90) return "A+";
            else if (avg >= 80) return "A";
            else if (avg >= 70) return "B";
            else if (avg >= 60) return "C";
            else if (avg >= 50) return "D";
            else return "F";
        }

        public void DisplayResult()
        {
            Console.WriteLine($"\nID:{Id}, Name:{Name}");
            Console.WriteLine($"Marks: {string.Join(", ", Marks)}");
            Console.WriteLine($"Average: {Cal_Avg():F2}");
            Console.WriteLine($"Grade:{GetGrade()}");
            Console.WriteLine("----------------------------");
        }
    }

    internal class Program
    {
        static string filePath = "students.json";

        static void Main()
        {
            List<Student> students = LoadData();
            bool running = true;

            while (running)
            {
                Console.WriteLine("\nStudent Result Processing");
                Console.WriteLine("1. Add Student");
                Console.WriteLine("2. View Results");
                Console.WriteLine("3. Search by ID");
                Console.WriteLine("4. Edit Marks");
                Console.WriteLine("5. Delete Student");
                Console.WriteLine("6. Save & Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddStudent(students);
                        break;

                    case "2":
                        if(!CheckIfEmpty(students))
                        {
                            foreach (var s in students)
                            {
                                s.DisplayResult();
                            }
                        }
                          
                        break; 

                    case "3":
                        if (!CheckIfEmpty(students))
                        {
                            foreach (var s in students)
                            {
                                SearchById(students);
                            }
                        }
                        break;

                    case "4":
                        if (!CheckIfEmpty(students))
                        {
                            foreach (var s in students)
                            {
                                EditMarks(students);
                            }
                        }
                        break;

                    case "5":
                        if (!CheckIfEmpty(students))
                        {
                            foreach (var s in students)
                            {
                                DeleteById(students);
                            }
                        }
                        break;

                    case "6":
                        SaveData(students);
                        running = false;
                        Console.WriteLine("Data saved. Exiting...");
                        break;

                    default: 
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }   
        }

        static bool CheckIfEmpty(List<Student> students)
        {
            if (students.Count == 0)
            {
                Console.WriteLine("No students found! File may be empty.");
                return true;
            }
            return false;
        }
        static void AddStudent(List<Student> students) 
        {
            Student s = new Student();
            Console.Write("Enter Student ID: ");
            int newId = Convert.ToInt32(Console.ReadLine());
            if (students.Exists(st => st.Id == newId))
            {
                Console.WriteLine($"A student with ID {newId} already exists. Please try again with a different ID.");
                return;
            }

            s.Id = newId;


            Console.Write("Enter Student Name: ");
            s.Name = Console.ReadLine();

            Console.Write("Enter number of subjects: ");
            int subjects = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < subjects; i++)
            {
                Console.Write($"Enter marks of subject {i + 1}: ");
                int mark = Convert.ToInt32(Console.ReadLine());
                s.Marks.Add(mark);
            }

            students.Add(s);
            Console.WriteLine("Student added successfully!");
        }

        static void SearchById(List<Student> students)
        {
            Console.Write("Enter the student Id to search: ");
            int id = Convert.ToInt32(Console.ReadLine());

            var student = students.Find(s=> s.Id == id);
            if(student != null)
            {
                student.DisplayResult();
            }
            else
            {
                Console.WriteLine("Student not found!");
            }
        }

        static void EditMarks(List<Student> students)
        {
            Console.WriteLine("Enter Student ID to edit: ");
            int id = Convert.ToInt32(Console.ReadLine());

            var student = students.Find(s=> s.Id == id);
            if (student == null)
            {
                Console.WriteLine("Student not found!");
                return;
            }

            Console.WriteLine("Current Marks: " + string.Join(", ", student.Marks));

            Console.Write("Enter new number of subjects: ");
            int subjects = Convert.ToInt32(Console.ReadLine());

            if(subjects > student.Marks.Count)
            {
                student.Marks.Clear();
                for(int i=0; i<=subjects; i++)
                {
                    Console.Write($"Enter marks for Subject {i+1}: ");
                    student.Marks.Add(Convert.ToInt32(Console.ReadLine()));
                }
                Console.WriteLine("Marks updated (all-re-entered).");
            }
            else if(subjects == student.Marks.Count)
            {
                Console.WriteLine("Choose the subject number to edit(1 - " + subjects + "): ");
                int subjectNumber = Convert.ToInt32(Console.ReadLine());

                if(subjectNumber >=1 && subjectNumber <= subjects)
                {
                    Console.Write("Enter new marks for Subject " + subjectNumber + ": ");
                    int newMark = Convert.ToInt32(Console.ReadLine());
                    student.Marks[subjectNumber - 1] = newMark;
                    Console.WriteLine("Marks updated successfully!");
                }
                else
                {
                    Console.WriteLine("Invalid subject number.");
                }
            }
            else
            {
                student.Marks.Clear();
                for (int i = 1; i <= subjects; i++)
                {
                    Console.Write($"Enter marks for Subject {i}: ");
                    student.Marks.Add(Convert.ToInt32(Console.ReadLine()));
                }
                Console.WriteLine("Marks updated (all re-entered due to fewer subjects).");
            }
        }

        static void DeleteById(List<Student> students)
        {
            Console.Write("Enter the Student ID to delete: ");
            int id = Convert.ToInt32(Console.ReadLine());

            var student = students.Find(s => s.Id == id);
            if(student == null)
            {
                Console.WriteLine("Student not found!");
                return;
            }
            students.Remove(student);
            Console.WriteLine($"Student with ID {id} has been deleted.");
        }

        static void SaveData(List<Student> students)
        {
            string json = JsonConvert.SerializeObject(students, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        static List<Student> LoadData()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<Student>>(json) ?? new List<Student>();
            }
            return new List<Student>();  
        }

    }
}
