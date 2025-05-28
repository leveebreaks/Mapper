using System.Linq.Expressions;
using AMapper;

namespace Main;

class Program
{
    static void Main(string[] args)
    {
        Mapper
            .ConfigureMap<A, B>()
            .AddMap(a => a.PropA, b => b.PropB, null)
            .AddMap(a => a.Prop2A, b => b.Prop2B, x => x.ToString());
        var a = new A();
        a.PropA = "asdas";
        a.Prop2A = 123;
        var b = Mapper.Map<B>(a);

        Console.WriteLine($"{b.Prop2B}, {b.PropB}");
    }

    public class A()
    {
        public string PropA { get; set; }
        public int Prop2A { get; set; }
    }

    public class B()
    {
        public string PropB { get; set; }
        public string Prop2B { get; set; }
    }
}