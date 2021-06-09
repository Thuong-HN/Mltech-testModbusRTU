using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace How_to_create_HMI_Control_Real_Time
{
    public partial class FormMain : Form
    {
        

        public FormMain()
        {
            InitializeComponent();
            ModbusRTUProtocol.Registers.Clear();
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40001 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40002 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40003 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40004 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40005 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40006 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40007 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40008 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40009 });
            ModbusRTUProtocol.Registers.Add(new Register() { Address = 40010 });
           // ModbusRTUProtocol.Registers.Add(new Register() { Address = 40011 });
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                ModbusRTUProtocol.Start();


                editorControl1.Register = ModbusRTUProtocol.Registers[0];
                editorControl2.Register = ModbusRTUProtocol.Registers[1];
                editorControl3.Register = ModbusRTUProtocol.Registers[2];
                editorControl4.Register = ModbusRTUProtocol.Registers[3];
                editorControl5.Register = ModbusRTUProtocol.Registers[4];
                editorControl6.Register = ModbusRTUProtocol.Registers[5];
                editorControl7.Register = ModbusRTUProtocol.Registers[6];
                editorControl8.Register = ModbusRTUProtocol.Registers[7];
                editorControl9.Register = ModbusRTUProtocol.Registers[8];
                editorControl10.Register = ModbusRTUProtocol.Registers[9];


                
                
                
                //ModbusRTUProtocol.WriteSingleRegister();

            }


            catch (Exception ex)
            {

            }

            //ModbusRTUProtocol.WriteSingleRegister(40001, ushort.Parse(editorControl1.Text));

        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                ModbusRTUProtocol.Stop();
                Application.DoEvents();
            } catch (Exception ex)
            {

            }
            
        }
    }
}
