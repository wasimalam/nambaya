package com.test.automation;

import java.io.File;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;
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


public class passwordChange {
	
	public Driver driver;

	  public static void main(String[] args) throws IOException {
			
		  passwordChange obj = new passwordChange();
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
              String passwordCardio= null;
			    String csvFile = "/Users/passwordChange.csv";
		        BufferedReader br = null;
		        String line = "";
		        String cvsSplitBy = ",";

		        try {

		            br = new BufferedReader(new FileReader(csvFile));
		            while ((line = br.readLine()) != null) {

		                // use comma as separator
		                String[] cardioData = line.split(cvsSplitBy);
		                userName=cardioData[0];
		                passwordCardio=cardioData[1];
		                

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
		        TimeUnit.SECONDS.sleep(3);
			    
			    
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
		        TimeUnit.SECONDS.sleep(3);
		        // Search Cardiologist by UserName
		        
		        //Search Box

		        TextEdit searchTextField = driver.find(TestProcess.class, new ProcessPattern(){{ 
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
		        searchTextField.click();
		        searchTextField.setwText(userName);
		        TimeUnit.SECONDS.sleep(3);
		        
		        
		        //Refresh Button 
		        

		        Button refreshButton = driver.find(TestProcess.class, new ProcessPattern(){{ 
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
		        refreshButton.click();
		        
		        
		        //Select Cardiologist Record
		        
		        
		        GridView userTable = driver.find(TestProcess.class, new ProcessPattern(){{ 
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
		        
		        userTable.clickCell(0,0);
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
