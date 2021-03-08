using System;
using dAuthMe.api.Models;
using MongoDB.Bson;

namespace dAuthMe.console
{
    class Program
    {
        static void Main()
        {
            var tmp = ObjectId.GenerateNewId();
            Console.WriteLine(tmp);
            Console.WriteLine(Guid.NewGuid().ToString().Replace("-", ""));
        }
    }
}
