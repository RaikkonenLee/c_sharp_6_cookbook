using System;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace PartialClassAddIn
{
    partial class frmInputs : Form
	{
		public frmInputs()
		{
			InitializeComponent();
		}

		private void frmInputs_Load(object sender, EventArgs e)
		{
			LoadAttribs();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (txtClassName.Text.Length > 0)
			{
				WriteClass();
				this.Close();
			}
			else
			{
				MessageBox.Show("Please enter a class name.");
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		public delegate void AddListboxItem(string name); 

		void AddItemToListBox(string name)
		{
			lbAttribs.Items.Add(name);
		}

		void LoadAttribs()
		{
			Type tAttrib = Type.GetType("System.Attribute");
			// load the assembly
			try
			{
				Assembly asm = Assembly.GetAssembly(tAttrib);
				Debug.WriteLine("Loaded " + asm.GetName().Name);
				foreach (Type asmType in asm.GetTypes())
				{
					if (asmType.BaseType?.Name == tAttrib.Name)
						// add to lb
						AddItemToListBox(asmType.ToString());
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
		}

		private void WriteClass()
		{
			string className = txtClassName.Text;
			string fileName = className + "_Attributes.cs";
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("// Using directives");
                sw.WriteLine("using System;");
                sw.WriteLine("");
                sw.WriteLine("namespace NamespaceFor{0}", className);
                sw.WriteLine("{");

                // writeout attribs
                WriteAttributes(sw);

                // write out class shell
                sw.WriteLine("	public partial class {0} ", className);
                sw.WriteLine("	{");
                sw.WriteLine("		public {0}()", className);
                sw.WriteLine("		{");
                sw.WriteLine("		}");
                sw.WriteLine("	}");
                sw.WriteLine("}");
            }

			// show this file in the IDE
			Process.Start(fileName);
		}

		void WriteAttributes(StreamWriter sw)
		{
			sw.WriteLine("	#region Attributes");
			for (int i = 0; i < lbAttribs.SelectedIndices.Count; i++)
			{
				string item = (string)lbAttribs.Items[lbAttribs.SelectedIndices[i]];
				Type t = Type.GetType(item);
				ConstructorInfo [] ci = t.GetConstructors();
				ParameterInfo [] pi = ci[0].GetParameters();
				// trim off "Attribute"
				item = item.Substring(0,item.LastIndexOf("Attribute",StringComparison.OrdinalIgnoreCase));
				sw.Write("	[{0}(", item);
				for(int p=0;p<pi.Length;p++)
				{
					Type pt = pi[p].ParameterType;
					switch (pt.FullName)
					{
						case "System.Byte":
							{
								sw.Write("0");
							}
							break;
						case "System.Boolean":
							{
								sw.Write("true");
							}
							break;
					}
					if (p != pi.Length - 1)
						sw.Write(",");
				}
				sw.WriteLine(")]");
			}
			sw.WriteLine("	#endregion // Attributes");
			sw.WriteLine("");
		}
	}
}