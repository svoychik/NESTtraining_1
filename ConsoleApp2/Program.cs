using Nest;
using System;

namespace ConsoleApp2
{
  partial class Program
  {


    private const string DefaultIndexName = "test";
    private const string ElasticSearchServerUri = @"http://localhost:9200";
    private const string UsersIndexName = "users";

    static void Main()
    {
      var client = CreateElasticClient();

      var users = new[] {
        new Сity { Code = "1", Name = "Київ" },
        new Сity { Code = "2", Name = "Житомир" },
        new Сity { Code = "3", Name = "Рівно" },
        new Сity { Code = "4", Name = "Володимир" },
        new Сity { Code = "5", Name = "Конотоп" },
        new Сity { Code = "6", Name = "Миколаїв" }
    };

      client.IndexMany(users);

      client.Refresh(UsersIndexName);

      var result = client.Search<Сity>(descriptor => descriptor
          .Query(q => q
              .QueryString(queryDescriptor => queryDescriptor
                  .Query("киї")
                  .Fields(fs => fs
                      .Fields(f1 => f1.Name)
                  )
              )
          )
      );
      Console.Read();
      // outputs 1
    }

    public static IElasticClient CreateElasticClient()
    {
      var settings = CreateConnectionSettings();
      var client = new ElasticClient(settings);

      if (client.IndexExists(UsersIndexName).Exists)
      {
        client.DeleteIndex(UsersIndexName);
      }

      client.CreateIndex(UsersIndexName, descriptor => descriptor
          .Mappings(ms => ms
              .Map<Сity>(m => m
                  .AutoMap()
              )
          )
          .Settings(s => s
              .Analysis(a => a
                  .Analyzers(analyzer => analyzer
                      .Custom("substring_analyzer", analyzerDescriptor => analyzerDescriptor
                          .Tokenizer("standard")
                          .Filters("lowercase", "substring")
                      )
                  )
                  .TokenFilters(tf => tf
                      .NGram("substring", filterDescriptor => filterDescriptor
                          .MinGram(2)
                          .MaxGram(15)
                      )
                  )
              )
          )
      );

      return client;
    }

    private static ConnectionSettings CreateConnectionSettings()
    {
      var uri = new Uri(ElasticSearchServerUri);
      var settings = new ConnectionSettings(uri)
          .DefaultIndex(DefaultIndexName)
          .InferMappingFor<Сity>(d => d
              .IndexName(UsersIndexName)
          );

      return settings;
    }
  }
}
