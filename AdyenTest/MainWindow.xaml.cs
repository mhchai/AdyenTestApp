using Adyen;
using Adyen.Model.Nexo.Message;
using Adyen.Security;
using Adyen.Service;
using System;
using System.Windows;

namespace AdyenTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Config _adyenConfig;
        private Client _adyenClient;
        private PosPaymentLocalApi _posPaymentLocalApi;
        private EncryptionCredentialDetails _encryptionCredentialDetails;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _adyenConfig = new Config
                {
                    Endpoint = EndPointTextBox.Text,
                    //Endpoint = "https://192.168.168.28:8443/nexo",
                    Environment = Adyen.Model.Enum.Environment.Test,
                    MerchantAccount = MerchantAccountTextBox.Text
                    //MerchantAccount = "Rashays"
                };

                _adyenClient = new Client(_adyenConfig);

                _posPaymentLocalApi = new PosPaymentLocalApi(_adyenClient);

                _encryptionCredentialDetails = new EncryptionCredentialDetails
                {
                    AdyenCryptoVersion = 1,
                    KeyIdentifier = IdentifierTextBox.Text,
                    //KeyIdentifier = "ChesterHillPos01",
                    Password = PassphaseTextBox.Text,
                    //Password = "RashaysChesterHillPos01",
                    KeyVersion = 1
                };

                displaytext.Text = "Initialization OK";
            }
            catch (Exception ex)
            {
                displaytext.Text = ex.Message;
            }
        }

        private void ReqPaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaleToPOIRequest salesRequest = new SaleToPOIRequest
                {
                    MessageHeader = new Adyen.Model.Nexo.MessageHeader
                    {
                        MessageClass = Adyen.Model.Nexo.MessageClassType.Service,
                        MessageCategory = Adyen.Model.Nexo.MessageCategoryType.Payment,
                        MessageType = Adyen.Model.Nexo.MessageType.Request,
                        ServiceID = DateTime.Now.ToString("yyMMddHHmm"),
                        SaleID = "POSSystemID12346",                                                
                        POIID = "P400Plus-805143973"
                    }
                };

                salesRequest.MessagePayload = new Adyen.Model.Nexo.PaymentRequest
                {
                    SaleData = new Adyen.Model.Nexo.SaleData
                    {
                        SaleTransactionID = new Adyen.Model.Nexo.TransactionIdentification
                        {
                            //TransactionID = "279088",
                            TransactionID = TransIdTextBox.Text,
                            TimeStamp = DateTime.UtcNow
                        }
                    },
                    PaymentTransaction = new Adyen.Model.Nexo.PaymentTransaction
                    {
                        AmountsReq = new Adyen.Model.Nexo.AmountsReq
                        {
                            //Currency = "EUR",
                            Currency = CurrencyTextBox.Text,
                            //RequestedAmount = 10.99M
                            RequestedAmount = Convert.ToDecimal(AmountTextBox.Text)
                        }
                    }
                };

                Adyen.Model.Nexo.SaleToPOIResponse paymentResponse = _posPaymentLocalApi.TerminalApiLocal(salesRequest, _encryptionCredentialDetails);

                if (paymentResponse.MessagePayload is Adyen.Model.Nexo.PaymentResponse nextpaymentresponse)
                {
                    displaytext.Text = $"Result : {nextpaymentresponse.Response.Result} - {nextpaymentresponse.Response.ErrorCondition}";
                }
            }
            catch (Exception ex)
            {
                displaytext.Text = "Payment Request Error : " + ex.Message;
            }
            
        }
    }
}
