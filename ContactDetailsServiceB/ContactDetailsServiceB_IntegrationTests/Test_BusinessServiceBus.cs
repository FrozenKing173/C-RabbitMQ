using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContactDetailsServiceB.BusinessModels;
using ContactDetailsServiceB.BusinessInterface;
using System;

/*
 * I'm initializing a Client and a Server to test end-to-end
 */

namespace ContactDetailsServiceB_IntegrationTests
{
    [TestClass]
    public class Test_BusinessServiceBus
    {
        IServiceBus businessClientContactDetails;
        IServiceBus businessServiceContactDetails;

        [TestInitialize]
        public void Initialize()
        {
            businessClientContactDetails = new BusinessServiceBus(1);
            businessServiceContactDetails = new BusinessServiceBus(2);
        }

        [TestMethod]
        public void Name_TestIsNullOrEmpty_ReturnsNotValidResponse()
        {
            string name = " ";
            string expecting = "  is not valid.";
            string actual = "";

            businessClientContactDetails.Send(name);
            
            actual = businessClientContactDetails.Receive();
            Assert.AreEqual(expecting, actual);
        }

        [TestMethod]
        public void Name_TestNotUpperCase_ReturnsNotValidResponse()
        {            
            string name = "jeandre";
            string expecting = "jeandre is not valid.";
            string actual = "";
             
            businessClientContactDetails.Send(name);
            
            actual = businessClientContactDetails.Receive();
            Assert.AreEqual(expecting, actual);
        }

        [TestMethod]
        public void Name_TestSpaces_ReturnsNotValidResponse()
        {           
            string name = "Jeandre van Dyk";
            string expecting = "Jeandre van Dyk is not valid.";
            string actual = "";

            businessClientContactDetails.Send(name);
            
            actual = businessClientContactDetails.Receive();
            Assert.AreEqual(expecting, actual);           
        }

        [TestMethod]
        public void Name_TestMaxCharacters_ReturnsNotValidResponse()
        {
            string name = "JeandrewenttoWongaforaninterviewandafteritwentwellhehadtowritethistest";
            string expecting = "JeandrewenttoWongaforaninterviewandafteritwentwellhehadtowritethistest is not valid.";
            string actual = "";

            businessClientContactDetails.Send(name);
            
            actual = businessClientContactDetails.Receive();
            Assert.AreEqual(expecting, actual);
        }

        [TestMethod]
        public void Name_TestSymbolOrNumber_ReturnsNotValidResponse()
        {
            string name = "Jeandre!173";
            string expecting = "Jeandre!173 is not valid.";
            string actual = "";

            businessClientContactDetails.Send(name);
            
            actual = businessClientContactDetails.Receive();
            Assert.AreEqual(expecting, actual);
        }

        [TestMethod]
        public void Name_TestOriginalName_ReturnsValidResponse()
        {
            string name = "Jeandre";
            string expecting = "Hello Jeandre, I am your father!";
            string actual = "";

            businessClientContactDetails.Send(name);

            actual = businessClientContactDetails.Receive();
            Assert.AreEqual(expecting, actual);
        }

        [TestCleanup]
        public void CloseBusinessServiceBus()
        {
            businessClientContactDetails.Close();
            businessServiceContactDetails.Close();
            GC.Collect();
        }
    }
}
