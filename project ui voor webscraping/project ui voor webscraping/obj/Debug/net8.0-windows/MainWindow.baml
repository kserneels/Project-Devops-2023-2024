   M S B A M L   `   `   `  ����  project ui voor webscraping    YourNamespace.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  League of Legends Scraper��5   	    � WindowState$ 	Maximized=�6+   .��5      � ���#272537 � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5	      $
��Auto �6   �� 5
      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��Center=�6"   $��Center=�6?   $��	0,20,0,20q�6Z   .+�5      �� $��League of Legends Scraper��6   
�������$��24(�6L   $
��Bold"�6Z   �� 5       �� Orientation$ 
Horizontal=�6   $��	0,10,0,10q�62   .+�5      �� $��Season:��6   
�������$��16(�6>   $��0,0,10,0q�6L   �� 5      -   5         ��Name$ SeasonTextBox��5      $	��250��61   $��16(�6=   5      �� 5      $ 
Horizontal=�6   $��0,0,0,10q�62   .+�5      �� $��Patch Number:��6   
�������$��16(�6D   $��0,0,10,0q�6R   �� 5      -   5        $ PatchTextBox��5      $	��250��60   $��16(�6<   5      5      �� 5   
   -   5        $��3��5      $��Scrape Data��6   $	��120��6M   $��10q�6Y   ��5      �� 5       �� 
TargetType" ��6   .�5       ��  �� Property � �� �� Value �	#FF2196F363   �� 5"       � ��$ 18(�61    �� Triggers 5$      P� 5%      	 P� Property	 �@J�IsMouseOver
 P� Value$

 True��69   . �5&      ��  � �� �	#FF1565C06;   �� 5(       �@�Cursor$
 Hand��67   5*      5,      5.      �� 54   
   $��4��6   $
��Auto=�6$   .��55      �� -   5         ��Name$ 
ResultText��55      
�������$��14(�6?   $�� ��6M   $��10q�6U   56      57      58      