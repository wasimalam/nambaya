package com.test.automation;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Properties;
import java.util.concurrent.TimeUnit;

import javax.imageio.ImageIO;

import java.awt.image.BufferedImage;
import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;

import com.smartbear.testleft.Driver;
import com.smartbear.testleft.LocalDriver;
import com.smartbear.testleft.Log;
import com.smartbear.testleft.testobjects.AWTPattern;
import com.smartbear.testleft.testobjects.Button;
import com.smartbear.testleft.testobjects.CheckBox;
import com.smartbear.testleft.testobjects.ComboBox;
import com.smartbear.testleft.testobjects.Control;
import com.smartbear.testleft.testobjects.GridView;
import com.smartbear.testleft.testobjects.ListBox;
import com.smartbear.testleft.testobjects.ProcessPattern;
import com.smartbear.testleft.testobjects.TestProcess;
import com.smartbear.testleft.testobjects.TopLevelWindow;
import com.smartbear.testleft.testobjects.TextEdit;
import com.smartbear.testleft.testobjects.WindowPattern;

public class NavigatorAutomation {

	public Driver driver;

	public static void main(String[] args) throws IOException {

		NavigatorAutomation obj = new NavigatorAutomation();
		try {
			if (args.length > 6) {
				obj.ProcessTest(args);
			}
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	public void ProcessTest(String[] args) throws Exception {
		// open Navigator
		Driver driver = new LocalDriver();
//		TestProcess process = driver.getApplications().run("C:\\Program Files\\Cardiac Navigator\\Navigator64.exe");
		for (int i = 0; i < args.length; i++) {
			System.out.println("Args " + i + " value : " + args[i]);
		}

//		Properties p = new Properties();
//		InputStream in = getClass().getResourceAsStream("config.properties");
//		p.load(in);
//		in.close();

		String dbUserName = args[0];
		String dbPassword = args[1];

		TimeUnit.SECONDS.sleep(2);
		// DB UsernNme
//		TextEdit dbUsernameElement = driver.find(TestProcess.class, new ProcessPattern() {
//			{
//				ProcessName = "Navigator64";
//				
//			}
//		}).find(TopLevelWindow.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.theme.tools.AuthDialog";
//				AWTComponentAccessibleName = "Database Login";
//				AWTComponentIndex = -1;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JRootPane";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.layeredPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.contentPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.theme.tools.FormPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(TextEdit.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JTextField";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		});
//
//		dbUsernameElement.click();
//		dbUsernameElement.setwText(dbUserName);
//		

		// db Password

//		Control dbPasswordTextBox = driver.find(TestProcess.class, new ProcessPattern() {
//			{
//				ProcessName = "Navigator64";
//			}
//		}).find(TopLevelWindow.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.theme.tools.AuthDialog";
//				AWTComponentAccessibleName = "Database Login";
//				AWTComponentIndex = -1;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JRootPane";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.layeredPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.contentPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.theme.tools.FormPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.theme.components.JPasswordFieldEx";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		});
//
//		dbPasswordTextBox.click();
//		
//		// String inputPassword = "ABC123ssi";
//		// dbPassword.setwText(inputPassword);
//		dbPasswordTextBox.keys(dbPassword);
//		
//
//		// DB Login Button Apply
//		Button applyButton = driver.find(TestProcess.class, new ProcessPattern() {
//			{
//				ProcessName = "Navigator64";
//			}
//		}).find(TopLevelWindow.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.theme.tools.AuthDialog";
//				AWTComponentAccessibleName = "Database Login";
//				AWTComponentIndex = -1;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JRootPane";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.layeredPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.contentPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Button.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JButton";
//				AWTComponentAccessibleName = "Apply";
//				AWTComponentIndex = 1;
//			}
//		});
//
//		applyButton.click();
//		TimeUnit.SECONDS.sleep(8);

		// Navigator Add Record Dropdown Button

		Control viewMenuLabel = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
				Index=1;
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.app.MainFrame";
				AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.CentralManager";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.ViewManager";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.ViewMenu";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 2;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "menu.db";
			}
		});

		viewMenuLabel.click();
		TimeUnit.SECONDS.sleep(2);
		// Import Records Button

		Control menuItemImportRecords = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.app.MainFrame";
				AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPopupMenu";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JMenuItem";
				AWTComponentAccessibleName = "Import Records";
				AWTComponentIndex = 2;
			}
		});

		menuItemImportRecords.click();

		// File Selection Menu

		Control metalFileChooserUI_3 = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JDialog";
				AWTComponentAccessibleName = "Import Records";
				AWTComponentIndex = -1;
				Index = 1;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.contentPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JFileChooser";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 2;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.plaf.metal.MetalFileChooserUI$3";
				AWTComponentAccessibleName = "File Name:";
				AWTComponentIndex = 0;
			}
		});

		metalFileChooserUI_3.click();
		TimeUnit.SECONDS.sleep(2);

		// Args[2]
		//String fileName = "C:\\NavigatorFiles\\EdfFiles\\_129.EDF";
		metalFileChooserUI_3.keys(args[2]);

		// Import EDF File button

		Button buttonImportEdf = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JDialog";
				AWTComponentAccessibleName = "Import Records";
				AWTComponentIndex = -1;
				Index = 1;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.contentPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JFileChooser";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 2;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 2;
			}
		}).find(Button.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JButton";
				AWTComponentAccessibleName = "Import Records";
				AWTComponentIndex = 0;
			}
		});

		buttonImportEdf.click();

		// args[]
		int fileReadDelay = Integer.parseInt(args[9]);
		TimeUnit.SECONDS.sleep(fileReadDelay);
		
		// Read CSV Patient

		// Args[3-6]
		String fName = args[3];
		String lName = args[4];
		String dob = args[5];
		String gender = args[6];
		// String csvFile = "/Users/patient.csv";
		// BufferedReader br = null;
		// String line = "";
		// String cvsSplitBy = ",";
		/*
		 * try {
		 * 
		 * br = new BufferedReader(new FileReader(csvFile)); while ((line =
		 * br.readLine()) != null) {
		 * 
		 * // use comma as separator String[] patientData = line.split(cvsSplitBy);
		 * fName=patientData[0]; lName=patientData[1]; dob=patientData[2]; }
		 * 
		 * } catch (FileNotFoundException e) { e.printStackTrace(); } catch (IOException
		 * e) { e.printStackTrace(); } finally { if (br != null) { try { br.close(); }
		 * catch (IOException e) { e.printStackTrace(); } } }
		 */
		
//		TopLevelWindow mainFrame = driver.find(TestProcess.class, new ProcessPattern(){{ 
//			ProcessName = "Navigator64"; 
//		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
//			JavaFullClassName = "com.cardiscope.app.MainFrame"; 
//			AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition"; 
//			AWTComponentIndex = 0; 
//		}});
//		
//		mainFrame.maximize();
		// Add A patient
		// Add Patient Button
		
		Control iconButtonAdd = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.app.MainFrame"; 
			AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition"; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JRootPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.layeredPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.CentralManager"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JPanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.ViewManager"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JPanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.ViewManagerPage"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.ViewManagerPage$1"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.impl.records.ViewRecordingList"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.theme.components.SidePanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.theme.components.IconButton"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}});
		iconButtonAdd.click();
		
		Control menuItemAdd = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.Popup$HeavyWeightWindow"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = -1; 
			Index = 1; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JRootPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.layeredPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.contentPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JPopupMenu"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JMenuItem"; 
			AWTComponentAccessibleName = "Create a new patient in database"; 
			AWTComponentIndex = 6; 
		}});
		menuItemAdd.click();
//		Control addPatientButton = driver.find(TestProcess.class, new ProcessPattern(){{ 
//			ProcessName = "Navigator64"; 
//		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
//			JavaFullClassName = "com.cardiscope.app.MainFrame"; 
//			AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition"; 
//			AWTComponentIndex = 0; 
//		}}).find(Control.class, new AWTPattern(){{ 
//			JavaFullClassName = "javax.swing.JRootPane"; 
//			AWTComponentAccessibleName = ""; 
//			AWTComponentIndex = 0; 
//		}}).find(Control.class, new AWTPattern(){{ 
//			AWTComponentName = "null.layeredPane"; 
//		}}).find(Control.class, new AWTPattern(){{ 
//			JavaFullClassName = "com.cardiscope.view.manager.CentralManager"; 
//			AWTComponentAccessibleName = ""; 
//			AWTComponentIndex = 0; 
//		}}).find(Control.class, new AWTPattern(){{ 
//			JavaFullClassName = "javax.swing.JPanel"; 
//			AWTComponentAccessibleName = ""; 
//			AWTComponentIndex = 0; 
//		}}).find(Control.class, new AWTPattern(){{ 
//			JavaFullClassName = "com.cardiscope.view.manager.ViewManager"; 
//			AWTComponentAccessibleName = ""; 
//			AWTComponentIndex = 0; 
//		}}).find(Control.class, new AWTPattern(){{ 
//			JavaFullClassName = "javax.swing.JPanel"; 
//			AWTComponentAccessibleName = ""; 
//			AWTComponentIndex = 0; 
//		}}).find(Control.class, new AWTPattern(){{ 
//			JavaFullClassName = "com.cardiscope.view.manager.ViewManagerPage"; 
//			AWTComponentAccessibleName = ""; 
//			AWTComponentIndex = 0; 
//		}}).find(Control.class, new AWTPattern(){{ 
//			JavaFullClassName = "com.cardiscope.view.manager.ViewManagerPage$1"; 
//			AWTComponentAccessibleName = ""; 
//			AWTComponentIndex = 0; 
//		}}).find(Control.class, new AWTPattern(){{ 
//			JavaFullClassName = "com.cardiscope.view.impl.records.ViewRecordingList"; 
//			AWTComponentAccessibleName = ""; 
//			AWTComponentIndex = 0; 
//		}}).find(Control.class, new AWTPattern(){{ 
//			JavaFullClassName = "com.cardiscope.theme.components.SidePanel"; 
//			AWTComponentAccessibleName = ""; 
//			AWTComponentIndex = 0; 
//		}}).find(Control.class, new AWTPattern(){{ 
//			JavaFullClassName = "com.cardiscope.theme.components.IconButton"; 
//			AWTComponentAccessibleName = ""; 
//			AWTComponentIndex = 7; 
//		}});
//
//		addPatientButton.click();
		//Patient Data
		//Patient ID 
		

		Control PatientIdtextFieldEx = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaCreateDialog"; 
			AWTComponentAccessibleName = "Create new patient"; 
			AWTComponentIndex = -1; 
			Index = 1; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JRootPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.layeredPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.contentPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.theme.tools.FormPanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.theme.components.JTextFieldEx"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}});
		TimeUnit.SECONDS.sleep(2);
		PatientIdtextFieldEx.hoverMouse();
		PatientIdtextFieldEx.keys(args[8]);
		
		// Enter Given Name/FirstName

		TextEdit textFieldFirstName = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaCreateDialog";
				AWTComponentAccessibleName = "Create new patient";
				AWTComponentIndex = -1;
				Index = 1;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.contentPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.theme.tools.FormPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(TextEdit.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JTextField";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		});
		textFieldFirstName.click();
		textFieldFirstName.setwText(fName);

		// Enter Family Name

		TextEdit textFieldLastName = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaCreateDialog";
				AWTComponentAccessibleName = "Create new patient";
				AWTComponentIndex = -1;
				Index=1;
				
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.contentPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.theme.tools.FormPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(TextEdit.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JTextField";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 1;
			}
		});
		textFieldLastName.click();
		textFieldLastName.setwText(lName);

		// Enter Date of Birth

		TextEdit formattedTextFieldDateofBirth = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaCreateDialog";
				AWTComponentAccessibleName = "Create new patient";
				AWTComponentIndex = -1;
				Index=1;
				
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.contentPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.theme.tools.FormPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(TextEdit.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JFormattedTextField";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		});
		
		dob=dob.replace("/", ".");
		formattedTextFieldDateofBirth.click();
		formattedTextFieldDateofBirth.setwText(dob);
		
		
		//Gender Dropdown
		
		Control metalComboBoxButton = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaCreateDialog"; 
			AWTComponentAccessibleName = "Create new patient"; 
			AWTComponentIndex = -1; 
			Index = 1; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JRootPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.layeredPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.contentPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.theme.tools.FormPanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(ComboBox.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JComboBox"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.plaf.metal.MetalComboBoxButton"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}});
		TimeUnit.SECONDS.sleep(2);
		metalComboBoxButton.click();
		TimeUnit.SECONDS.sleep(2);		
		
		//Select Gender
		
		ListBox genderList = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.Popup$HeavyWeightWindow"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = -1; 
			Index = 1; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JRootPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.layeredPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.contentPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "ComboPopup.popup"; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "ComboBox.scrollPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JViewport"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(ListBox.class, new AWTPattern(){{ 
			AWTComponentName = "ComboBox.list"; 
		}});

		
		if(args[6].contentEquals("M"))
		{
			genderList.clickItem(1);
		}
		else if(args[6].contentEquals("F"))
		{
		genderList.clickItem(2);
		}
		else
		{
		genderList.clickItem(0);	
		}
		
		
		

		// Create Patient Button

		Button buttonCreatePatient = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaCreateDialog";
				AWTComponentAccessibleName = "Create new patient";
				AWTComponentIndex = -1;
				Index=1;
				
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.contentPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 1;
			}
		}).find(Button.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JButton";
				AWTComponentAccessibleName = "Create";
				AWTComponentIndex = 2;
			}
		});

		buttonCreatePatient.click();

		// Assign Patient to a Record

		// Select Record
		Control viewRecordingList_RecordRowRenderer = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.app.MainFrame";
				AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.CentralManager";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.ViewManager";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.ViewManagerPage";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.ViewManagerPage$1";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.records.ViewRecordingList";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.records.ViewRecordingList$2";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.records.ViewRecordingList$RecordRowRenderer";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		});
		TimeUnit.SECONDS.sleep(3);
		viewRecordingList_RecordRowRenderer.click();

		// Assign Button

//		Control AssignPatientButton = driver.find(TestProcess.class, new ProcessPattern() {
//			{
//				ProcessName = "Navigator64";
//			}
//		}).find(TopLevelWindow.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.app.MainFrame";
//				AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JRootPane";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.layeredPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.view.manager.CentralManager";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.view.manager.ViewManager";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.view.manager.ViewManagerPage";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.view.manager.ViewManagerPage$1";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.view.impl.records.ViewRecordingList";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.theme.components.SidePanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.theme.components.IconButton";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 5;
//			}
//		});
//		TimeUnit.SECONDS.sleep(3);
//		AssignPatientButton.click();
		TimeUnit.SECONDS.sleep(3);
		iconButtonAdd.click();
		Control menuItemAssign = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.Popup$HeavyWeightWindow"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = -1; 
			Index = 1; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JRootPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.layeredPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.contentPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JPopupMenu"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JMenuItem"; 
			AWTComponentAccessibleName = "Assign recording to patient"; 
			AWTComponentIndex = 4; 
		}});
		
		menuItemAssign.click();
		
		// Search Patient
		// Enter Patient Firstname

		Control searchPatient = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaSelector";
				AWTComponentAccessibleName = "Patient selection";
				AWTComponentIndex = -1;
				Index = 1;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.contentPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaSelectionPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.theme.components.JTextFieldEx";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		});

		searchPatient.click();
		searchPatient.keys(args[8]);
		TimeUnit.SECONDS.sleep(2);

		// Select Patient Record from Table

		GridView tablePatient = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaSelector";
				AWTComponentAccessibleName = "Patient selection";
				AWTComponentIndex = -1;
				Index = 1;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.contentPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JScrollPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JViewport";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(GridView.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JTable";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		});

		tablePatient.clickCell(0,0);
		TimeUnit.SECONDS.sleep(3);

		// Select Button

		Button buttonSelect = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaSelector";
				AWTComponentAccessibleName = "Patient selection";
				AWTComponentIndex = -1;
				Index = 1;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.contentPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaSelectionPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Button.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JButton";
				AWTComponentAccessibleName = "Select";
				AWTComponentIndex = 2;
			}
		});

		buttonSelect.click();
		

		// Confirm Assignment of Record

		Button buttonConfirm = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JDialog";
				AWTComponentAccessibleName = "Assign recording to patient";
				AWTComponentIndex = -1;
				Index = 1;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.contentPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JOptionPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "OptionPane.buttonArea";
			}
		}).find(Button.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JButton";
				AWTComponentAccessibleName = "Yes";
				AWTComponentIndex = 0;
			}
		});

		buttonConfirm.click();
		TimeUnit.SECONDS.sleep(2);
		
		// Open QT Dashboard.

		viewRecordingList_RecordRowRenderer.dblClick();
		TimeUnit.SECONDS.sleep(3);

		// Go to QT Section

		Control viewMenuQT = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.app.MainFrame";
				AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.CentralManager";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.ViewManager";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 2;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.ViewMenu";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 1;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "20";
			}
		});

		viewMenuQT.click();
		Control centralMenuLabelClose = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
			Index = 1; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.app.MainFrame"; 
			AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition"; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JRootPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.layeredPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.CentralManager"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.CentralMenu"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.CentralMenuLabel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 2; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.CentralMenuLabelClose"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}});
		
		centralMenuLabelClose.hoverMouse();
		TimeUnit.SECONDS.sleep(2);

		// Qt ScreenCapture
		
		// QTC By Severity

		Control qtcSeverity = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.app.MainFrame";
				AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.CentralManager";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.ViewManager";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 2;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JSplitPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.ViewManagerPage";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 7;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.piechart.ViewParaCountPieChart";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		});

		// QTC by Distribution

		Control qtcDistribution = driver.find(TestProcess.class, new ProcessPattern() {
			{
				ProcessName = "Navigator64";
			}
		}).find(TopLevelWindow.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.app.MainFrame";
				AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JRootPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				AWTComponentName = "null.layeredPane";
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.CentralManager";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.ViewManager";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 2;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JSplitPane";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "javax.swing.JPanel";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.manager.ViewManagerPage";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 7;
			}
		}).find(Control.class, new AWTPattern() {
			{
				JavaFullClassName = "com.cardiscope.view.impl.histogram.ViewHRVHistogram";
				AWTComponentAccessibleName = "";
				AWTComponentIndex = 0;
			}
		});
		
		//QTC Stat
		
		Control QTC = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.app.MainFrame"; 
			AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition"; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JRootPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.layeredPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.CentralManager"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JPanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.ViewManager"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 2; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JSplitPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JPanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.ViewManagerPage"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 7; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.impl.info.ViewHRVBox"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}});
	

				//ScreenShots QTC 
		
		try {
	        String format = "png";
	        String fileNameQT = args[7]; 
	        
	        File file = new File(fileNameQT);
	        if(!file.exists())
	        {
	        	//file.mkdirs();
	        }
	        
	        BufferedImage QTCD = qtcDistribution.picture();
	        BufferedImage QTCS = qtcSeverity.picture();
	        BufferedImage QTCStats = QTC.picture();
	        String fileNameQTCD=fileNameQT + "\\" + args[8] + "_QTCD." + format;
	        String fileNameQTCS=fileNameQT + "\\" + args[8] + "_QTCS." + format;
	        String fileNameQTC=fileNameQT + "\\" + args[8] + "_QTC." + format;
	        ImageIO.write(QTCD, format, new File(fileNameQTCD));
	        ImageIO.write(QTCS, format, new File(fileNameQTCS));
	        
	        //Append Images into 1
	        int widthQTC1=QTCStats.getWidth();
	        int widthQTC2=QTCS.getWidth();
	        int widthQTC3=QTCD.getWidth();
	        int height= QTCS.getHeight();
	        
	        
	        
	        BufferedImage img = new BufferedImage(widthQTC1+widthQTC2+widthQTC3,height,BufferedImage.TYPE_INT_RGB);
	        
	        boolean image1Drawn = img.createGraphics().drawImage(QTCStats, 0, 0, null); // 0, 0 are the x and y positions
	         //where we are placing image1 in final image

	        boolean image2Drawn = img.createGraphics().drawImage(QTCS, widthQTC1, 0, null); // here width is mentioned as width of
	        
	        boolean image3Drawn = img.createGraphics().drawImage(QTCD, widthQTC1+widthQTC2, 0, null); // here width is mentioned as width of
	        
	        
	        ImageIO.write(img, format, new File(fileNameQTC));
	        
	        
	        
	        
	        
	        
	        
	        
	        System.out.printf("The screenshot was saved!");
	    } catch (IOException ex) {
	        System.err.println(ex);
	    }		
		TimeUnit.SECONDS.sleep(1);
		centralMenuLabelClose.click();
		
		//Patient Details Close Button
		

		Control PatientMenuLabelClose = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
			Index = 1; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.app.MainFrame"; 
			AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition"; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JRootPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.layeredPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.CentralManager"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.CentralMenu"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.CentralMenuLabel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 1; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.manager.CentralMenuLabelClose"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}});
		PatientMenuLabelClose.click();
	//	process.close();
		
		
		
		
		
		// Printing Report

//		// Print Button
//
//		Control viewMenuPrint = driver.find(TestProcess.class, new ProcessPattern() {
//			{
//				ProcessName = "Navigator64";
//			}
//		}).find(TopLevelWindow.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.app.MainFrame";
//				AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JRootPane";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.layeredPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.view.manager.CentralManager";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.view.manager.ViewManager";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 2;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.view.manager.ViewMenu";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 2;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "menu.print";
//			}
//		});
//
//		viewMenuPrint.click();
//		TimeUnit.SECONDS.sleep(3);
//		// Create Report Button
//
//		Control menuItemCreateReport = driver.find(TestProcess.class, new ProcessPattern() {
//			{
//				ProcessName = "Navigator64";
//			}
//		}).find(TopLevelWindow.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.app.MainFrame";
//				AWTComponentAccessibleName = "Bittium Cardiac Navigator - Professional Edition";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JRootPane";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.layeredPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JPopupMenu";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JMenuItem";
//				AWTComponentAccessibleName = "Create a report";
//				AWTComponentIndex = 0;
//			}
//		});
//
//		menuItemCreateReport.click();
//
//		// Long Term ECG Report Select
//
//		ListBox list = driver.find(TestProcess.class, new ProcessPattern() {
//			{
//				ProcessName = "Navigator64";
//			}
//		}).find(TopLevelWindow.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.report.CreateReportDialog";
//				AWTComponentAccessibleName = "Create a report";
//				AWTComponentIndex = -1;
//				Index = 1;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JRootPane";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.layeredPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.contentPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 1;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.report.CreateReportDialog$ReportList";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JViewport";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(ListBox.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JList";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		});
//		TimeUnit.SECONDS.sleep(3);
//		list.dblClickItem(3);
//
//		// Check BOx QT FInal
//
//		CheckBox checkBoxQt = driver.find(TestProcess.class, new ProcessPattern() {
//			{
//				ProcessName = "Navigator64";
//			}
//		}).find(TopLevelWindow.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.theme.tools.ConfigDialog";
//				AWTComponentAccessibleName = "Long-term ECG Report";
//				AWTComponentIndex = -1;
//				Index = 1;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JRootPane";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.layeredPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.contentPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 1;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.theme.tools.FormPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(CheckBox.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JCheckBox";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 9;
//			}
//		});
//
//		checkBoxQt.click();
//		TimeUnit.SECONDS.sleep(3);
//		// Apply Button
//
//		Button buttonApply = driver.find(TestProcess.class, new ProcessPattern() {
//			{
//				ProcessName = "Navigator64";
//			}
//		}).find(TopLevelWindow.class, new AWTPattern() {
//			{
//				JavaFullClassName = "com.cardiscope.theme.tools.ConfigDialog";
//				AWTComponentAccessibleName = "Long-term ECG Report";
//				AWTComponentIndex = -1;
//				Index = 1;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JRootPane";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.layeredPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.contentPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 2;
//			}
//		}).find(Button.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JButton";
//				AWTComponentAccessibleName = "Apply";
//				AWTComponentIndex = 4;
//			}
//		});
//
//		buttonApply.click();
//		TimeUnit.SECONDS.sleep(7);
//
//		// Save Report
//
//		Button buttonSaveReport = driver.find(TestProcess.class, new ProcessPattern() {
//			{
//				ProcessName = "Navigator64";
//			}
//		}).find(TopLevelWindow.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JDialog";
//				AWTComponentAccessibleName = "Store PDF report to folder";
//				AWTComponentIndex = -1;
//				Index = 1;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JRootPane";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.layeredPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				AWTComponentName = "null.contentPane";
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JFileChooser";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 0;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 2;
//			}
//		}).find(Control.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JPanel";
//				AWTComponentAccessibleName = "";
//				AWTComponentIndex = 2;
//			}
//		}).find(Button.class, new AWTPattern() {
//			{
//				JavaFullClassName = "javax.swing.JButton";
//				AWTComponentAccessibleName = "Save";
//				AWTComponentIndex = 0;
//			}
//		});
//
//		buttonSaveReport.click();

	}
}
