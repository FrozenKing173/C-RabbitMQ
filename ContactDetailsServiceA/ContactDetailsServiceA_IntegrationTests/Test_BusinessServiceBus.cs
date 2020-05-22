using Xunit;
using ContactDetailsServiceA.BusinessModels;
using ContactDetailsServiceA.BusinessInterfaces;
using System;

/*
 * I'm initializing a Client and a Server to test end-to-end
 */

namespace ContactDetailsServiceA_IntegrationTests
{
    public class Test_BusinessServiceBus
    {
        // Act as sut
        private readonly IServiceBus _businessClientContactDetails;

        // Act as Fixture
        private readonly IServiceBus _businessServiceContactDetails;


        public Test_BusinessServiceBus()
        {
            _businessClientContactDetails = new BusinessServiceBus(1);
            _businessServiceContactDetails = new BusinessServiceBus(2);
        }

        [Fact]
        public void Name_TestIsNullOrEmpty_ReturnsNotValidResponse()
        {
            string name = " ";
            string expecting = "  is not valid.";
            string actual = "";

            _businessClientContactDetails.Send(name);
            
            actual = _businessClientContactDetails.Receive();
            Assert.Equal(expecting, actual);
        }

        [Fact]
        public void Name_TestNotUpperCase_ReturnsNotValidResponse()
        {            
            string name = "jeandre";
            string expecting = "jeandre is not valid.";
            string actual = "";
             
            _businessClientContactDetails.Send(name);
            
            actual = _businessClientContactDetails.Receive();
            Assert.Equal(expecting, actual);
        }

        [Fact]
        public void Name_TestSpaces_ReturnsNotValidResponse()
        {           
            string name = "Jeandre van Dyk";
            string expecting = "Jeandre van Dyk is not valid.";
            string actual = "";

            _businessClientContactDetails.Send(name);
            
            actual = _businessClientContactDetails.Receive();
            Assert.Equal(expecting, actual);           
        }

        [Fact]
        public void Name_TestMaxCharacters_ReturnsNotValidResponse()
        {
            string name = "JeandrewenttoWongaforaninterviewandafteritwentwellhehadtowritethistest";
            string expecting = "JeandrewenttoWongaforaninterviewandafteritwentwellhehadtowritethistest is not valid.";
            string actual = "";

            _businessClientContactDetails.Send(name);
            
            actual = _businessClientContactDetails.Receive();
            Assert.Equal(expecting, actual);
        }

        [Fact]
        public void Name_TestSymbolOrNumber_ReturnsNotValidResponse()
        {
            string name = "Jeandre!173";
            string expecting = "Jeandre!173 is not valid.";
            string actual = "";

            _businessClientContactDetails.Send(name);
            
            actual = _businessClientContactDetails.Receive();
            Assert.Equal(expecting, actual);
        }

        [Fact]
        public void Name_TestOriginalName_ReturnsValidResponse()
        {
            string name = "Jeandre";
            string expecting = "Hello Jeandre, I am your father!";
            string actual = "";

            _businessClientContactDetails.Send(name);

            actual = _businessClientContactDetails.Receive();
            Assert.Equal(expecting, actual);
        }

        [Fact]
        public void CloseBusinessServiceBus()
        {
            _businessClientContactDetails.Close();
            _businessServiceContactDetails.Close();
            GC.Collect();
        }
    }
}
