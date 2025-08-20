package com.test.automation;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Properties;
import java.util.concurrent.TimeUnit;

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
import com.smartbear.testleft.testobjects.Control;
import com.smartbear.testleft.testobjects.GridView;
import com.smartbear.testleft.testobjects.ListBox;
import com.smartbear.testleft.testobjects.ProcessPattern;
import com.smartbear.testleft.testobjects.TestProcess;
import com.smartbear.testleft.testobjects.TopLevelWindow;
import com.smartbear.testleft.testobjects.TextEdit;
import com.smartbear.testleft.testobjects.WindowPattern;

public class TestCase {

	public Driver driver;

	public static void main(String[] args) throws IOException {
		  
		try {
			TestCase obj = new TestCase();
			if (args.length > 6) {
				obj.ProcessTest(args);
			}
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public void ProcessTest(String[] args) throws Exception {
		//open Nav
		Driver driver = new LocalDriver();
		TestProcess process = driver.getApplications().run("C:\\Program Files\\Cardiac Navigator\\Navigator64.exe");
		
		for (int i = 0; i < args.length; i++) {
			System.out.println("Args " + i + " value : " + args[i]);
		}
		  
		TextEdit textField = driver.find(TestProcess.class, new ProcessPattern(){{ 
		  	ProcessName = "Navigator64"; 
		  }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		  	JavaFullClassName = "com.cardiscope.theme.tools.AuthDialog"; 
		  	AWTComponentAccessibleName = "Database Encryption"; 
		  	AWTComponentIndex = -1; 
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
		  }}).find(TextEdit.class, new AWTPattern(){{ 
		  	JavaFullClassName = "javax.swing.JTextField"; 
		  	AWTComponentAccessibleName = ""; 
		  	AWTComponentIndex = 0; 
		  }});
		  
		  textField.click();
		  textField.setwText(args[0]);
	  
	
	
	// db Password
		  Control passwordFieldEx = driver.find(TestProcess.class, new ProcessPattern(){{ 
				ProcessName = "Navigator64"; 
			}}).find(TopLevelWindow.class, new AWTPattern(){{ 
				JavaFullClassName = "com.cardiscope.theme.tools.AuthDialog"; 
				AWTComponentAccessibleName = "Database Encryption"; 
				AWTComponentIndex = -1; 
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
				JavaFullClassName = "com.cardiscope.theme.components.JPasswordFieldEx"; 
				AWTComponentAccessibleName = ""; 
				AWTComponentIndex = 0; 
			}});

		  passwordFieldEx.click();
		  passwordFieldEx.keys(args[1]);
		  
		  //Apply Button 
		  Button buttonApply = driver.find(TestProcess.class, new ProcessPattern(){{ 
				ProcessName = "Navigator64"; 
			}}).find(TopLevelWindow.class, new AWTPattern(){{ 
				JavaFullClassName = "com.cardiscope.theme.tools.AuthDialog"; 
				AWTComponentAccessibleName = "Database Encryption"; 
				AWTComponentIndex = -1; 
			}}).find(Control.class, new AWTPattern(){{ 
				JavaFullClassName = "javax.swing.JRootPane"; 
				AWTComponentAccessibleName = ""; 
				AWTComponentIndex = 0; 
			}}).find(Control.class, new AWTPattern(){{ 
				AWTComponentName = "null.layeredPane"; 
			}}).find(Control.class, new AWTPattern(){{ 
				AWTComponentName = "null.contentPane"; 
			}}).find(Control.class, new AWTPattern(){{ 
				JavaFullClassName = "javax.swing.JPanel"; 
				AWTComponentAccessibleName = ""; 
				AWTComponentIndex = 0; 
			}}).find(Button.class, new AWTPattern(){{ 
				JavaFullClassName = "javax.swing.JButton"; 
				AWTComponentAccessibleName = "Apply"; 
				AWTComponentIndex = 1; 
			}});
		  buttonApply.click();
		  
		  
		  //ButtonContinueTrial
	  Button buttonContinue = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JDialog"; 
			AWTComponentAccessibleName = "Attention"; 
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
			JavaFullClassName = "javax.swing.JOptionPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "OptionPane.buttonArea"; 
		}}).find(Button.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JButton"; 
			AWTComponentAccessibleName = "continue without license"; 
			AWTComponentIndex = 0; 
		}});
	  
	  buttonContinue.click();
	  TimeUnit.SECONDS.sleep(3);
	  
	  
	  //DB Button CLick
	  Control viewMenuLabelDB = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.app.MainFrame"; 
			AWTComponentAccessibleName = "Bittium Cardiac Navigator - Viewer Edition"; 
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
			JavaFullClassName = "com.cardiscope.view.manager.ViewMenu"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JPanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 2; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "menu.db"; 
		}});
	  
	  viewMenuLabelDB.click();	  
	  
	  //Import Records	  
	  Control menuItemImport = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.app.MainFrame"; 
			AWTComponentAccessibleName = "Bittium Cardiac Navigator - Viewer Edition"; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JRootPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "null.layeredPane"; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JPanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JPopupMenu"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JMenuItem"; 
			AWTComponentAccessibleName = "Import Records"; 
			AWTComponentIndex = 2; 
		}});
	  
	  menuItemImport.click();
	  // Enter File ID
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
	metalFileChooserUI_3.keys(args[2]);
	  
	  //Import Button 
	
	Button buttonImport = driver.find(TestProcess.class, new ProcessPattern(){{ 
		ProcessName = "Navigator64"; 
	}}).find(TopLevelWindow.class, new AWTPattern(){{ 
		JavaFullClassName = "javax.swing.JDialog"; 
		AWTComponentAccessibleName = "Import Records"; 
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
		JavaFullClassName = "javax.swing.JFileChooser"; 
		AWTComponentAccessibleName = ""; 
		AWTComponentIndex = 0; 
	}}).find(Control.class, new AWTPattern(){{ 
		JavaFullClassName = "javax.swing.JPanel"; 
		AWTComponentAccessibleName = ""; 
		AWTComponentIndex = 2; 
	}}).find(Control.class, new AWTPattern(){{ 
		JavaFullClassName = "javax.swing.JPanel"; 
		AWTComponentAccessibleName = ""; 
		AWTComponentIndex = 2; 
	}}).find(Button.class, new AWTPattern(){{ 
		JavaFullClassName = "javax.swing.JButton"; 
		AWTComponentAccessibleName = "Import Records"; 
		AWTComponentIndex = 0; 
	}});
	  
	  buttonImport.click();
	  
	  TimeUnit.MINUTES.sleep(1);
	  
	  Control iconButtonAdd = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.app.MainFrame"; 
			AWTComponentAccessibleName = "Bittium Cardiac Navigator - Viewer Edition"; 
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
			AWTComponentIndex = 7; 
		}});
	  
	  iconButtonAdd.click();

		// Enter Given Name/FirstName

	  TextEdit textFieldFirstName = driver.find(TestProcess.class, new ProcessPattern(){{ 
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
		}}).find(TextEdit.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JTextField"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}});
		textFieldFirstName.click();
		TimeUnit.SECONDS.sleep(2);
		textFieldFirstName.setwText(args[3]);

		// Enter Family Name

		TextEdit textFieldLastName = driver.find(TestProcess.class, new ProcessPattern(){{ 
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
		}}).find(TextEdit.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JTextField"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 1; 
		}});
		TimeUnit.SECONDS.sleep(2);
		textFieldLastName.click();
		textFieldLastName.setwText(args[4]);

		// Enter Date of Birth

		TextEdit formattedTextFieldDateofBirth = driver.find(TestProcess.class, new ProcessPattern(){{ 
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
		}}).find(TextEdit.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JFormattedTextField"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}});
		TimeUnit.SECONDS.sleep(1);
		formattedTextFieldDateofBirth.click();
		formattedTextFieldDateofBirth.setwText(args[5]);
		TimeUnit.SECONDS.sleep(1);

		// Create Patient Button

		Button buttonCreatePatient = driver.find(TestProcess.class, new ProcessPattern(){{ 
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
			JavaFullClassName = "javax.swing.JPanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 1; 
		}}).find(Button.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JButton"; 
			AWTComponentAccessibleName = "Create"; 
			AWTComponentIndex = 2; 
		}});

		buttonCreatePatient.click();
	  
	  
	  //Record Attach Patient
		Control viewRecordingList_RecordRowRenderer = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.app.MainFrame"; 
			AWTComponentAccessibleName = "Bittium Cardiac Navigator - Viewer Edition"; 
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
			JavaFullClassName = "com.cardiscope.view.impl.records.ViewRecordingList$2"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.impl.records.ViewRecordingList$RecordRowRenderer"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}});
	  
		TimeUnit.SECONDS.sleep(3);
		viewRecordingList_RecordRowRenderer.click(0,0);
		
		
		//Attach Patient
		Control iconButtonAttach = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.app.MainFrame"; 
			AWTComponentAccessibleName = "Bittium Cardiac Navigator - Viewer Edition"; 
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
			AWTComponentIndex = 5; 
		}});
		
		TimeUnit.SECONDS.sleep(3);
		iconButtonAttach.click();
		
		//Search Bar
		
		Control textFieldEx = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaSelector"; 
			AWTComponentAccessibleName = "Patient selection"; 
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
			JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaSelectionPanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.theme.components.JTextFieldEx"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}});
		textFieldEx.click();
		TimeUnit.SECONDS.sleep(3);
		textFieldEx.keys(args[3]);
		TimeUnit.SECONDS.sleep(3);
		
		
		GridView table = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaSelector"; 
			AWTComponentAccessibleName = "Patient selection"; 
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
			JavaFullClassName = "javax.swing.JPanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JScrollPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JViewport"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(GridView.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JTable"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}});
		
		table.clickCell(0,0);
		
		//Select Button CLick
		Button buttonSelect = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaSelector"; 
			AWTComponentAccessibleName = "Patient selection"; 
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
			JavaFullClassName = "com.cardiscope.view.impl.persona.PersonaSelectionPanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JPanel"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Button.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JButton"; 
			AWTComponentAccessibleName = "Select"; 
			AWTComponentIndex = 2; 
		}});
		
		buttonSelect.click();
		
		
		//Confirm assignment
		
		Button buttonConfirm = driver.find(TestProcess.class, new ProcessPattern(){{ 
			ProcessName = "Navigator64"; 
		}}).find(TopLevelWindow.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JDialog"; 
			AWTComponentAccessibleName = "Assign recording to patient"; 
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
			JavaFullClassName = "javax.swing.JOptionPane"; 
			AWTComponentAccessibleName = ""; 
			AWTComponentIndex = 0; 
		}}).find(Control.class, new AWTPattern(){{ 
			AWTComponentName = "OptionPane.buttonArea"; 
		}}).find(Button.class, new AWTPattern(){{ 
			JavaFullClassName = "javax.swing.JButton"; 
			AWTComponentAccessibleName = "Yes"; 
			AWTComponentIndex = 0; 
		}});
		buttonConfirm.click();
		TimeUnit.SECONDS.sleep(4);
		viewRecordingList_RecordRowRenderer.dblClick();	
		TimeUnit.SECONDS.sleep(4);
		
		process.close();
		
		
		
		
		
	}
}