using Checker.Queries;

namespace Checker
{
    public class LispValidatorTest
    {
        [Theory]
        [MemberData(nameof(QueryParam))]        
        public void LispValidationTest(List<string> instance, string id)
        {
            //var verify = new LispValidator();

            //var fileName = Directory.GetFiles(@"./Repo").Where(f => f.Contains($"{id}")).FirstOrDefault();

            //instance = (List<string>)File.ReadLines(fileName);

            //var isValid = verify.LispValidation(instance, id);
            
            //Assert.True(isValid);
        }
    }
}