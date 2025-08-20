package com.test.automation;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Properties;
import java.util.concurrent.TimeUnit;

import javax.imageio.ImageIO;

import java.awt.Rectangle;
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
import com.smartbear.testleft.testobjects.Control;
import com.smartbear.testleft.testobjects.GridView;
import com.smartbear.testleft.testobjects.ListBox;
import com.smartbear.testleft.testobjects.ProcessPattern;
import com.smartbear.testleft.testobjects.TestProcess;
import com.smartbear.testleft.testobjects.TopLevelWindow;
import com.smartbear.testleft.testobjects.TextEdit;
import com.smartbear.testleft.testobjects.WindowPattern;

public class ScreenTest {

	public Driver driver;

	public static void main(String[] args) throws IOException {
		  
		try {
			ScreenTest obj = new ScreenTest();
			if (args.length > 6) {
				obj.ProcessTest(args);
			}
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	public void ProcessTest(String[] args) throws Exception{
		
		// Make Session
//		Runtime rt = Runtime.getRuntime();
//		Process pr = rt.exec("SessionCreator.exe StartRestServer /UserName:Administrator /password:2404$HetznerServer%2020 /UseActiveSession");
		
		//open Nav
				Driver driver = new LocalDriver();
				//TestProcess process = driver.getApplications().run("C:\\Program Files\\Cardiac Navigator\\Navigator64.exe");
				
				for (int i = 0; i < args.length; i++) {
					System.out.println("Args " + i + " value : " + args[i]);
				}
				  
				
				

				
			

				

				Control viewParaCountPieChart = driver.find(TestProcess.class, new ProcessPattern(){{ 
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
					JavaFullClassName = "com.cardiscope.view.impl.piechart.ViewParaCountPieChart"; 
					AWTComponentAccessibleName = ""; 
					AWTComponentIndex = 0; 
				}});
			 
				
				
				//Rectangle rect = viewDBInfo.getBounds();
				 
			    try {
			        String format = "png";
			        String fileName = args[7] +"\\"+ args[3] ;
			        File file = new File(fileName);
			        if(!file.exists())
			        {
			        	file.mkdirs();
			        }
			        String fileName2=fileName + "\\"+ "QTCD." + format;
			        //BufferedImage Img1= viewHRVBox.picture();
			        BufferedImage Img2 = viewParaCountPieChart.picture();
			        
			        //int width1= Img1.getWidth();
			        //int height1= Img1.getHeight();
			        int width2= Img2.getWidth();
			        int height2= Img2.getHeight();
			        
			        BufferedImage img = new BufferedImage(width2,0,BufferedImage.TYPE_INT_RGB);
			        
			        // boolean image1Drawn = img.createGraphics().drawImage(Img1, 0, 0, null); // 0, 0 are the x and y positions
			         //where we are placing image1 in final image

			        //boolean image2Drawn = img.createGraphics().drawImage(Img2, width1, 0, null); // here width is mentioned as width of
			        
			        
			        
			        
			        
			        
			        
			        ImageIO.write(img, format, new File (fileName2));
			 
			        System.out.printf("The screenshot was saved!");
			    } catch (IOException ex) {
			        System.err.println(ex);
			    }
			  
	}
	
	
	
	
	
}