   M S B A M L   `   `   `  ���� Y  ULabMan_WPF_VialSimulator_Naive, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullO,clr-namespace:LabMan_WPF_VialSimulator_NaiveLabMan_WPF_VialSimulator_Naive  /    )LabMan_WPF_VialSimulator_Naive.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     >local,clr-namespace:LabMan_WPF_VialSimulator_Naive     -     � Title$(  "Vial Simulator V0.1 - D.Fitzgerald��$	��400��$	��800�� � WindowState$ 	Maximized=� � SizeToContent$ Manual=�.���  � ColumnDefinitions �� $��5* ��� $
��0.5* ��� $��5* ��� $
��0.5* �.+��� $��0�� �� Orientation$ Vertical=�.+��  �� $��2* ��� $��2* ��� $��* � � RowDefinitions �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* �.+��� $��0��$��0��$��2 0 0 2q�Input Rack Capacity�� -    ��Name$ TextBox_InputRackCapacity��$��1��$��0��$��0 0 0 2q� �� 
IsReadOnly ��  �� Path$ 	IsRunning�	 �� Mode$	 TwoWay=����� $, &SimulationParameters.InputRackCapacity�$	 OneWay=��� $��0��$��1��$��2 0 0 2q�Output Rack Capacity�� -   $ ��TextBox_OutputRackCapacity��$��1��$��1��$��0 0 0 2q� �� $ 	IsRunning�$	 TwoWay=����� $- 'SimulationParameters.OutputRackCapacity�$	 OneWay=��� $��0��$��2��$��2 0 0 2q�Output Division Factor�� -   $��TextBox_OutputDivison��$��1��$��2��$��0 0 0 2q� �� $ 	IsRunning�$	 TwoWay=����� $/ )SimulationParameters.OutputDivisionFactor�$	 OneWay=��� $��0��$��3��$��2 0 0 2q�Balance Error�� -   $��TextBox_BalanceError��$��1��$��3��$��0 0 0 2q�$
 true��
����������� $* $SimulationParameters.BalanceError_mg�$	 OneWay=��� $��2��$��3��$��2 0 0 2q�mg�� $��0��$��4��$��2 0 0 2q�Target Output Weight�� -   $��TextBox_OutputWeight��$��1��$��4��$��0 0 0 2q� �� $ 	IsRunning�$	 TwoWay=����� $4 .SimulationParameters.TargetOutputVialWeight_mg�$	 OneWay=��� $��2��$��4��$��2 0 0 2q�mg�� $��0��$��5��$��2 0 0 2q�Dispenser Flow Rate�� -   $��TextBox_DispenserFlowRate��$��1��$��5��$��0 0 0 2q� �� $ 	IsRunning�$	 TwoWay=����� $0 *SimulationParameters.DispenserFlowRate_mgs�$	 OneWay=��� $��2��$��5��$��2 0 0 2q�mg/s�� $ 
Horizontal=�.+��� -   $��Button_Simulate��$��10 0 0 0q�$��70��$��30��{��� $ 	IsStopped�.��
Simulate�� -	   $��Button_Stop��$��10 0 0 0q�$��70��$��30��{��� $ 	IsRunning�.��Stop�� -
   $��Button_Export��$��10 0 0 0q�$��70��$��30��{��� $ 	CanExport�.��Export .csv�� $ Vertical=�.+��� 
Input Rack� .+��� -   
 ��Name$
 RackTemplate_Input��   InputRackTemplate_Level1#���   �� Output Rack� .+��� -   $
 RackTemplate_Output��  OutputRackTemplate_Level1#���  �� $��2��$ Vertical=�.+�]� $��Stretch=�$��Stretch=�.+��� $	��Top=�Selected Vial Info�� -   $��TextBlock_VialInfo�����#EEEEEE$
 true��$��Bottom=�$��Stretch=�$��Stretch=�$
��auto��$	��100��]� $��Stretch=�$��Stretch=�.+��  �� $��* ��� $��* � �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* �.+��� $��0��$��0��$��2 0 0 2q�Arm Position�� -   $ TextBox_ArmPosition��$��1��$��0��$��0 0 0 2q�$
 true������ $ Arm.ArmPosition�$	 TwoWay=��� $��0��$��1��$��2 0 0 2q�
Arm Status�� -   $ TextBox_ArmStatus��$��1��$��1��$��0 0 0 2q�$
 true������ $ Arm.ArmStatus�$	 TwoWay=��� $��0��$��2��$��2 0 0 2q�Grip Position�� -   $ TextBox_GripPosition��$��1��$��2��$��0 0 0 2q�$
 true������ $ Arm.GripPosition�$	 TwoWay=��� $��0��$��3��$��2 0 0 2q�Grip Status�� -   $ TextBox_GripStatus��$��1��$��3��$��0 0 0 2q�$
 true������ $ Arm.GripStatus�$	 TwoWay=��� $��0��$��4��$��2 0 0 2q�Currently Held�� -   $ TextBox_HoldingStatus��$��1��$��4��$��0 0 0 2q�$
 true������ $ Arm.HoldingStatus�$	 TwoWay=��� $��0��$��5��$��2 0 0 2q�Grind Status�� -   $ TextBox_GrindStatus��$��1��$��5��$��0 0 0 2q�$
 true������ $ GrindStation.GrindStatus�$	 TwoWay=��� $��0��$��6��$��2 0 0 2q�Dispenser Status�� -   $ TextBox_DispenerStatus��$��1��$��6��$��0 0 0 2q�$
 true������ $% DispenseStation.DispenserStatus�$	 TwoWay=�]� $��Stretch=�$��Stretch=�.+��� $	��Top=�Debug Output�� -   $��TextBlock_DebugOutput�����#EEEEEE$
 true��$��Bottom=�$��Stretch=�$��Stretch=�$
��auto��$	��350��$
��Auto=�$
��Auto=�$
��True�� � 	Resources   InputRackTemplate_Level2  OutputRackTemplate_Level2��%  &	       &	  �     &	 �     &	 �    �� .R��� -   $��50��$��50��$��1 1 1 1q�.���� .+��� ���� $ ID��� $ 
Horizontal=�.+��� ���� $ Weight��� $��mg���� ���� $ State��� .R��� #���  ���� ���� .R��� $ 
Horizontal=��� .R��� -   $��50��$��70��$��1 1 1 1q�.���� .+��� ���� $ ID��� $ 
Horizontal=�.+��� ���� $ Weight��� $��mg���� ���� $ State��� .R��� #���  ���� ���� .R��� $ 
Horizontal=�