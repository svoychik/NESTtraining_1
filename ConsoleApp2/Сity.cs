using Nest;

namespace ConsoleApp2
{
  partial class Program
  {
    public class Сity
    {
      [Text(Analyzer = "substring_analyzer")]
      public string Code { get; set; }
      [Text(Analyzer = "substring_analyzer")]
      public string Name { get; set; }
    }
  }
}
