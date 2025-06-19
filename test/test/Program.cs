using System.Text;

namespace test;

class Program
{
    static void Main(string[] args)
    {
        List<int> list = [ 1, 2, 3 ];

        var foo = list.Where(x => x == 2);

        var foo1 = list.Where(x => x == 2).ToList();

        foreach (var item in foo)
        {
            Console.WriteLine(item);
        }

        var sb = new StringBuilder();



        string str1 = "hello";

        string str2 = StrMethod(str1);

        //Console.WriteLine($"str1 {str1} --- str2 {str2}");
        Console.WriteLine(Equals(str1, str2));
    }

    public static string StrMethod(string str)
    {
        str = str.ToUpper();
        return str;
    }



    public class Animal
    {
        private int age;

        public int Age
        {
            get { return age; }
            set { age = value; }
        }


        public void GetAge()
        {
            Console.WriteLine($"Age:{age}");
        }

        public Animal(int age)
        {
            Age = age;
        }
    }

    public class Cat : Animal
    {
        public Cat(int age) : base(age)
        {
            Age = age * 4;
        }
    }

    public class Dog : Animal
    {
        public Dog(int age) : base(age)
        {
            Age = age * 2;
        }
    }

    public Cat cat = new Cat(4);

    public Dog dog = new Dog(5);

}