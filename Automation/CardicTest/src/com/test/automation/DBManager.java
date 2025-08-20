package com.test.automation;

import java.io.File;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.concurrent.TimeUnit;

import org.apache.http.util.Args;

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
import com.smartbear.testleft.testobjects.TabControl;
import com.smartbear.testleft.testobjects.TestProcess;
import com.smartbear.testleft.testobjects.TopLevelWindow;
import com.smartbear.testleft.testobjects.TextEdit;
import com.smartbear.testleft.testobjects.WindowPattern;
import com.smartbear.testleft.testobjects.swing.JMenuBar;

public class DBManager {

	public Driver driver;

	  public static void main(String[] args) throws IOException {
			
		  DBManager obj = new DBManager();
			try {
				obj.ProcessTest();
			} catch (Exception e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}

	  
	  public void ProcessTest() throws Exception {
			//open DB Manager
		  Driver driver = new LocalDriver();
			    TestProcess process = driver.getApplications().run("C:\\Program Files\\farosdb\\csdb-admin64.exe");
	 
			    TimeUnit.SECONDS.sleep(10);
			    
			    //Database Management Host 

			    

			    TextEdit dbHost = driver.find(TestProcess.class, new ProcessPattern(){{ 
			    	ProcessName = "csdb-admin64"; 
			    }}).find(TopLevelWindow.class, new AWTPattern(){{ 
			    	JavaFullClassName = "com.cardiscope.db.mgnt.AuthDialog"; 
			    	AWTComponentAccessibleName = "Database Manager Log-in"; 
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
			    }}).find(TextEdit.class, new AWTPattern(){{ 
			    	JavaFullClassName = "javax.swing.JTextField"; 
			    	AWTComponentAccessibleName = ""; 
			    	AWTComponentIndex = 0; 
			    }});
			    dbHost.click();
			    String inputText = "172.18.1.61";
			    dbHost.setwText(inputText);
			    TimeUnit.SECONDS.sleep(3);
			    
			    //Username DB 
			    

			    TextEdit dbUserName = driver.find(TestProcess.class, new ProcessPattern(){{ 
			    	ProcessName = "csdb-admin64"; 
			    	Index = 1; 
			    }}).find(TopLevelWindow.class, new AWTPattern(){{ 
			    	JavaFullClassName = "com.cardiscope.db.mgnt.AuthDialog"; 
			    	AWTComponentAccessibleName = "Database Manager Log-in"; 
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
			    }}).find(TextEdit.class, new AWTPattern(){{ 
			    	JavaFullClassName = "javax.swing.JTextField"; 
			    	AWTComponentAccessibleName = ""; 
			    	AWTComponentIndex = 1; 
			    }});
			    dbUserName.dblClick();
			    dbUserName.setwText("sysadmin");
			    TimeUnit.SECONDS.sleep(3);
			   
			    //DB Password
			   

			    TextEdit dbPassword = driver.find(TestProcess.class, new ProcessPattern(){{ 
			    	ProcessName = "csdb-admin64"; 
			    	Index = 1; 
			    }}).find(TopLevelWindow.class, new AWTPattern(){{ 
			    	JavaFullClassName = "com.cardiscope.db.mgnt.AuthDialog"; 
			    	AWTComponentAccessibleName = "Database Manager Log-in"; 
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
			    }}).find(TextEdit.class, new AWTPattern(){{ 
			    	JavaFullClassName = "javax.swing.JPasswordField"; 
			    	AWTComponentAccessibleName = ""; 
			    	AWTComponentIndex = 0; 
			    }});
			    dbPassword.click();
			    dbPassword.keys("sysadmin");
			    TimeUnit.SECONDS.sleep(3);
			    
			    
			    // Login DB Button
			 

			    Button buttonLogin = driver.find(TestProcess.class, new ProcessPattern(){{ 
			    	ProcessName = "csdb-admin64"; 
			    }}).find(TopLevelWindow.class, new AWTPattern(){{ 
			    	JavaFullClassName = "com.cardiscope.db.mgnt.AuthDialog"; 
			    	AWTComponentAccessibleName = "Database Manager Log-in"; 
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
			    	AWTComponentAccessibleName = "Login"; 
			    	AWTComponentIndex = 0; 
			    }});
			    
			    buttonLogin.click();
			    TimeUnit.SECONDS.sleep(20);
			    
			    //Add new User (Cardiologist)
			    
			    // Read Cardio Credentials from CSV file
			    
			    String userName = null;
                String name = null;
                String passwordCardio= null;
			    String csvFile = "/Users/cardio.csv";
		        BufferedReader br = null;
		        String line = "";
		        String cvsSplitBy = ",";

		        try {

		            br = new BufferedReader(new FileReader(csvFile));
		            while ((line = br.readLine()) != null) {

		                // use comma as separator
		                String[] cardioData = line.split(cvsSplitBy);
		                userName=cardioData[0];
		                name=cardioData[1];
		                passwordCardio=cardioData[2];

		            }

		        } catch (FileNotFoundException e) {
		            e.printStackTrace();
		        } catch (IOException e) {
		            e.printStackTrace();
		        } finally {
		            if (br != null) {
		                try {
		                    br.close();
		                } catch (IOException e) {
		                    e.printStackTrace();
		                }
		            }
		        }

			    // Click Worksheets Menu
		        
		        
		        Control menuWorksheets = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.DatabaseConsole"; 
		        	AWTComponentAccessibleName = "Database Manager 1.1.60 :: 172.18.1.61"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JRootPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.layeredPane"; 
		        }}).find(JMenuBar.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JMenuBar"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JMenu"; 
		        	AWTComponentAccessibleName = "Worksheets"; 
		        	AWTComponentIndex = 0; 
		        }});
		        
		        menuWorksheets.click();
		        
			    
			    
			    // User Management Button
			   

		        Control menuItem = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.DatabaseConsole"; 
		        	AWTComponentAccessibleName = "Database Manager 1.1.60 :: 172.18.1.61"; 
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
		        	AWTComponentAccessibleName = "User management"; 
		        	AWTComponentIndex = 0; 
		        }});
		        menuItem.click();
		        
		        
		        //New User Button
		        
		        Button buttonNew = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.DatabaseConsole"; 
		        	AWTComponentAccessibleName = "Database Manager 1.1.60 :: 172.18.1.61"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JRootPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.layeredPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.DatabaseConsole$DesktopPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JInternalFrame"; 
		        	AWTComponentAccessibleName = "Users"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JRootPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.layeredPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.contentPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserListPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Button.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JButton"; 
		        	AWTComponentAccessibleName = "New"; 
		        	AWTComponentIndex = 2; 
		        }});
		        
			    
		        buttonNew.click();
		        
		      
		     // Enter User Details Here
		     // Enter User Details Here
		     
		        //Enter UserName
		       

		        TextEdit userNametext = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserEditDialog"; 
		        	AWTComponentAccessibleName = "User Manager - Edit"; 
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
		        }}).find(TabControl.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JTabbedPane"; 
		        	AWTComponentAccessibleName = "General"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(TextEdit.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JTextField"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }});
		        
		        userNametext.click();
		        userNametext.setwText(userName);
		        
		        
		        //Set Name
		        
		        

		        TextEdit nameTextField = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserEditDialog"; 
		        	AWTComponentAccessibleName = "User Manager - Edit"; 
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
		        }}).find(TabControl.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JTabbedPane"; 
		        	AWTComponentAccessibleName = "General"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(TextEdit.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JTextField"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 1; 
		        }});
		        TimeUnit.SECONDS.sleep(3);
		        nameTextField.click();
		        nameTextField.setwText(name);
		        TimeUnit.SECONDS.sleep(3);
		        
		        
		        
		        
		        
		        
		        //Access Tab Access

		        TabControl tabbedPane = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserEditDialog"; 
		        	AWTComponentAccessibleName = "User Manager - Edit"; 
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
		        }}).find(TabControl.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JTabbedPane"; 
		        	AWTComponentAccessibleName = "General"; 
		        	AWTComponentIndex = 0; 
		        }});
		        
		        tabbedPane.clickTab(1);
		        
		        
		        
		     
		        
		        
		        
		     //User Access Settings
		       
		    

		        Button buttonAddAccess = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserEditDialog"; 
		        	AWTComponentAccessibleName = "User Manager - Edit"; 
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
		        }}).find(TabControl.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JTabbedPane"; 
		        	AWTComponentAccessibleName = "Access"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 1; 
		        }}).find(Button.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JButton"; 
		        	AWTComponentAccessibleName = "Add"; 
		        	AWTComponentIndex = 1; 
		        }});   
		        
		        buttonAddAccess.click();
		        
		        
		        
		        
		        //Options Menu Settings 
		        
		        TimeUnit.SECONDS.sleep(3);
		        tabbedPane.clickTab(2);
		        Control metalComboBoxButton = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserEditDialog"; 
		        	AWTComponentAccessibleName = "User Manager - Edit"; 
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
		        }}).find(TabControl.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JTabbedPane"; 
		        	AWTComponentAccessibleName = "Options"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 2; 
		        }}).find(ComboBox.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JComboBox"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.plaf.metal.MetalComboBoxButton"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }});
		        
		        
		        metalComboBoxButton.click();
		        
		        
		        
		        
		        
		        
		        
		        
		        //Select Privileged User 

		        ListBox basicComboPopup_1 = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserEditDialog"; 
		        	AWTComponentAccessibleName = "User Manager - Edit"; 
		        	AWTComponentIndex = -1; 
		        	Index = 1; 
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
		        
		        TimeUnit.SECONDS.sleep(3);
		        basicComboPopup_1.clickItem(4);
		        TimeUnit.SECONDS.sleep(3);
		        
		        
		        //CheckBoxes
		        
		        //General Access Check Box
		        

		        CheckBox generalAccessCheckBox = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserEditDialog"; 
		        	AWTComponentAccessibleName = "User Manager - Edit"; 
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
		        }}).find(TabControl.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JTabbedPane"; 
		        	AWTComponentAccessibleName = "Options"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 2; 
		        }}).find(CheckBox.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JCheckBox"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }});
		        
		        generalAccessCheckBox.click();
		        TimeUnit.SECONDS.sleep(3);
		        
		        
		        //Multi Session CheckBox
		        

		        CheckBox multiSessionCheckBox = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserEditDialog"; 
		        	AWTComponentAccessibleName = "User Manager - Edit"; 
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
		        }}).find(TabControl.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JTabbedPane"; 
		        	AWTComponentAccessibleName = "Options"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 2; 
		        }}).find(CheckBox.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JCheckBox"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 1; 
		        }});
		        
		        multiSessionCheckBox.click();
		        
		        //Auto Login CheckBox
		        
		        

		        CheckBox autoLoginCheckBox = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserEditDialog"; 
		        	AWTComponentAccessibleName = "User Manager - Edit"; 
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
		        }}).find(TabControl.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JTabbedPane"; 
		        	AWTComponentAccessibleName = "Options"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 2; 
		        }}).find(CheckBox.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JCheckBox"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 2; 
		        }});
		        
		        autoLoginCheckBox.click();
		        
		        
		        //Issue Requests CheckBox
		        

		        CheckBox issueRequestsCheckBox = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserEditDialog"; 
		        	AWTComponentAccessibleName = "User Manager - Edit"; 
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
		        }}).find(TabControl.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JTabbedPane"; 
		        	AWTComponentAccessibleName = "Options"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 2; 
		        }}).find(CheckBox.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JCheckBox"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 3; 
		        }});
		        
		        issueRequestsCheckBox.click();
		        TimeUnit.SECONDS.sleep(3);
		        
		        
		        //Create User Button 
		        
		        Button createUserButton = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserEditDialog"; 
		        	AWTComponentAccessibleName = "User Manager - Edit"; 
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
		        }}).find(Button.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JButton"; 
		        	AWTComponentAccessibleName = "Create"; 
		        	AWTComponentIndex = 0; 
		        }});
		        
		        createUserButton.click();
		        TimeUnit.SECONDS.sleep(3);
		        
		        
		        
		        //Set Password///////// 
		        
		        //Search User 
		        

		        TextEdit SearchtextField = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.DatabaseConsole"; 
		        	AWTComponentAccessibleName = "Database Manager 1.1.60 :: 172.18.1.61"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JRootPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.layeredPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.DatabaseConsole$DesktopPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JInternalFrame"; 
		        	AWTComponentAccessibleName = "Users"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JRootPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.layeredPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.contentPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserListPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 1; 
		        }}).find(TextEdit.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JTextField"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }});
		        TimeUnit.SECONDS.sleep(3);
		        SearchtextField.click();
		        SearchtextField.setwText(userName);
		        
		        
		        //Click Refresh Button 
		        

		        Button Refreshbutton = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.DatabaseConsole"; 
		        	AWTComponentAccessibleName = "Database Manager 1.1.60 :: 172.18.1.61"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JRootPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.layeredPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.DatabaseConsole$DesktopPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JInternalFrame"; 
		        	AWTComponentAccessibleName = "Users"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JRootPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.layeredPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.contentPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserListPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Button.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JButton"; 
		        	AWTComponentAccessibleName = "Refresh"; 
		        	AWTComponentIndex = 3; 
		        }});
		        
		        Refreshbutton.click();
		        
		        //Select Record 
		       
		        GridView table = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.DatabaseConsole"; 
		        	AWTComponentAccessibleName = "Database Manager 1.1.60 :: 172.18.1.61"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JRootPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.layeredPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.DatabaseConsole$DesktopPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JInternalFrame"; 
		        	AWTComponentAccessibleName = "Users"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JRootPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.layeredPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.contentPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserListPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.json.utils.JSONTable"; 
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
		        TimeUnit.SECONDS.sleep(3);
		        //Click Password
		        
		        

		        Button Passwordbutton = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.DatabaseConsole"; 
		        	AWTComponentAccessibleName = "Database Manager 1.1.60 :: 172.18.1.61"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JRootPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.layeredPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.DatabaseConsole$DesktopPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JInternalFrame"; 
		        	AWTComponentAccessibleName = "Users"; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JRootPane"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.layeredPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	AWTComponentName = "null.contentPane"; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserListPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Control.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JPanel"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }}).find(Button.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JButton"; 
		        	AWTComponentAccessibleName = "Password"; 
		        	AWTComponentIndex = 1; 
		        }});
		        
		        Passwordbutton.click();
		        TimeUnit.SECONDS.sleep(3);
		        
		        //Set Password
		        
		        

		        TextEdit PasswordtextField = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserPwdDialog"; 
		        	AWTComponentAccessibleName = "User Manager - Password"; 
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
		        }}).find(TextEdit.class, new AWTPattern(){{ 
		        	JavaFullClassName = "javax.swing.JTextField"; 
		        	AWTComponentAccessibleName = ""; 
		        	AWTComponentIndex = 0; 
		        }});
		        
		        PasswordtextField.click();
		        PasswordtextField.setwText(passwordCardio);
		        TimeUnit.SECONDS.sleep(3);
		        
		        
		        //Set Password
		        
		        

		        Button SetPasswordbutton = driver.find(TestProcess.class, new ProcessPattern(){{ 
		        	ProcessName = "csdb-admin64"; 
		        }}).find(TopLevelWindow.class, new AWTPattern(){{ 
		        	JavaFullClassName = "com.cardiscope.db.mgnt.data.UserPwdDialog"; 
		        	AWTComponentAccessibleName = "User Manager - Password"; 
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
		        	AWTComponentAccessibleName = "Set"; 
		        	AWTComponentIndex = 2; 
		        }});
		        
		        
		        SetPasswordbutton.click();
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
		        
			    
	  }
}