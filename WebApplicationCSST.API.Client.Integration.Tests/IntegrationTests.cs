using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WebApplicationCSST.API.Client.Integration.Tests
{
    public class IntegrationTests
    {
        [Test]
        public void Test1_EmptyArgument_ThrowException()
        {
            Exception ex = Assert.ThrowsAsync<ArgumentException>(async () => await Program.Main(new string[] { }));

            Assert.That(ex.Message.Equals("No arguments."));
        }

        [Test]
        public async Task Test2_ValidArgument_ReturnOutputOnConsole()
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                // Act
                await Program.Main(new string[] { "1" });
                
                var resultOutput = sw.ToString();

                Assert.That(resultOutput.Contains("Name :"));
            }
        }
    }
}