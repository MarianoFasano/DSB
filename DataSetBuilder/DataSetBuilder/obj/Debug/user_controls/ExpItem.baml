   M S B A M L   `   `   `  ����I  EDataSetBuilder, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:DataSetBuilder.user_controlsDataSetBuilder.user_controls  5      *    $DataSetBuilder.user_controls.ExpItem   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:DataSetBuilder.user_controls     5      .��5	      � -   5          �Name$  ExpGrid��5	       � RowDefinitions 5
   
   �� 5      �� 5       � ColumnDefinitions 5   
   �� 5      �� 5      .+�5   
   ]� -   5         ]�Name$ DepoDockPanel��5      $��0��6+   $��0��6;   ��. .+�5      �� -   5         ��Name$ DepoSearchBox��5   +   $	��Top=�6   $��20��6k    �� TextWrapping$
 Wrap=�6w   o� 5      -   5        $��Modifica��5   +   $��Bottom=�6   .@�5      m� $��Opzioni��6   ��6,   ��  �� ElementName$ Modifica�� �� Path$ ActualWidth�.@�5      m� -   5         m�Name$ ViewDetailsMenu��5      $��Vedi dettagli��68   6O   m� 5      -   5        $ EditComment��5      $��Modifica Commento��64   6O   m� 5      -   5        $ UpdateDepoView��5      $��Aggiorna��67   6I   m� 5      -   5        $ CompressDepo��5      $��Comprimi��65   6G   m� 5      -	   5        $ DeleteDepoMenu��5      $��Cancella deposizione��67   6U   5      5      �� 5      -
   5        	 ��Name$	 DepoDetails��5      $��Bottom=�6.   $y�	Collapsed=�6F   .+�5       �� $��1q�6   
���OO/�._�5!      �� -   5        
 ��Name$
 Provino��5!      $
��Fill=�6-   5"      -� 5#      -   5         -�Name$ DepoComment��5#   %   5$      �� 5%      -   5         ��Name$ DepositionViewer��5%      $
��auto��60   5&      5'      �� 5(   
   -   5        $ 
LiveStatus��5(      $��0��6&   $��1��63   5)      �� 5*   
   $��0��6   $��0��6$   $��4��64   $��Stretch=�6H   $��Bottom=�6f   $��2��6�   
�������6�   �� 5+   
   $��1��6   $
��Left=�6'   $��Stretch=�6B   $��2��6^   
�������6h   5,      5-      