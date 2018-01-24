using System;
using Xbehave;
using Xunit;
using PFML.BusinessLogic.Test.Utilities;
using Moq;
using PFML.BusinessLogic.Test.Logging;

namespace PFML.BusinessLogic.Test.TestScripts.Core.Headers.AgencyLogic
{
    public class Search
    {
        public int Add(int x, int y) => x + y;
    }

    /// <summary>
    /// Sample BO.
    /// </summary>
    public class Person
    {
        public int PersonId { get; set; }
        public int Age { get; set; }
    }

    /// <summary>
    /// Sample data source.
    /// </summary>
    public interface IDataSource
    {
        Person GetUser(int userId);
    }

    /// <summary>
    /// Sample business logic.
    /// </summary>
    public class BusinessLogic
    {
        private readonly IDataSource _dataSource;

        /// <summary>
        /// Datasource can be anything MSSQL, Oracle or even XML.
        /// </summary>
        /// <param name="dataSource"></param>
        public BusinessLogic(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        /// <summary>
        /// Business rule.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns>bool</returns>
        public bool PersonCanDrink(int personId)
        {
            //Database call
            var person = _dataSource.GetUser(personId);
            return person.Age >= 21;
        }

    }

    /// <summary>
    /// Sample test class.
    /// </summary>
    public class SampleTestWithXBehave : IClassFixture<TestFixture>
    {
        //xBehaviour example.
        [Scenario]
        [Example(1, 2, 3)]
        [Example(2, 3, 5)]
        public void Addition(int x, int y, int expectedAnswer, Search calculator, int answer)
        {
            $"Given the number {x}"
                .x(() => { });

            $"And the number {y}"
                .x(() => { });

            "And a calculator"
                .x(() => calculator = new Search());

            "When I add the numbers together"
                .x(() => answer = calculator.Add(x, y));

            $"Then the answer is {answer}"
                .x(() => Assert.Equal(expectedAnswer, answer));

            //log.Error("Application started");
        }


        //xUnit example.
        [Theory]
        [InlineData(50, true)]
        [InlineData(51, true)]
        [InlineData(19, false)]
        public void UserCanDrink_UserIsOfAge_ReturnsTrue(int age, bool expectedResult)
        {
            //******arrange******
            //No need of actual datasource, just mock it.
            var dataSource = new Mock<IDataSource>();
            var person = new Person() { Age = age };

            //In reality this function (GetUser) gets the user from datasource. 
            //But here we dont really need to get it from the datasource.
            dataSource.Setup(d => d.GetUser(default(int))).Returns(person);

            BusinessLogic bl = new BusinessLogic(dataSource.Object);

            //******act******
            var result = bl.PersonCanDrink(default(int));
            //LogHelper.Log("Person age is " + age.ToString() + " and can drink is " + result.ToString() + Environment.NewLine);
            LogHelper.Log(LoggingLevel.Info, "Person age is " + age.ToString() + " and can drink is " + result.ToString(), null, this);

            try
            {
                int x = 0;
                var res = 5 / x;
            }
            catch (Exception ex)
            {
                LogHelper.Log(LoggingLevel.Error, string.Empty, ex, this);
                throw;
            }

            //******assert******
            Assert.Equal(expectedResult, result);
        }
    }
}