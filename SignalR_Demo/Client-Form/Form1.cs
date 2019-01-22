using Microsoft.AspNet.SignalR.Client;
using System;
using System.Windows.Forms;

// Install-Package Microsoft.AspNet.SignalR.Client

namespace Client_Form {
    public partial class Form1 : Form
    {
        public HubConnection con;
        public IHubProxy hub;

        public Form1() {
            InitializeComponent();
            OnLoad();
        }

        private void OnLoad() {
            this.con = new HubConnection("http://localhost:8888/");
            
            this.hub = con.CreateHubProxy("MyHub");
            hub.On("onMessageAdded", (msg) => {                
                lstMessages.BeginInvoke(new Action(() => {
                    lstMessages.Items.Add(msg);
                }));
            });

            con.Start().Wait();
            lblStatus.Text = "CONNECTED";
        }

        private void btnSend_Click(object sender, EventArgs e) {
            hub.Invoke("AddMessage", txtMessage.Text);
            txtMessage.Text = "";
        }
    }
}
