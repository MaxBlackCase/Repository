namespace SortAlgorithms {
  partial class Form1 {
    /// <summary>
    /// Обязательная переменная конструктора.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Освободить все используемые ресурсы.
    /// </summary>
    /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
    protected override void Dispose( bool disposing ) {
      if ( disposing && (components != null) )
      {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Код, автоматически созданный конструктором форм Windows

    /// <summary>
    /// Требуемый метод для поддержки конструктора — не изменяйте 
    /// содержимое этого метода с помощью редактора кода.
    /// </summary>
    private void InitializeComponent() {
      this.panel1 = new System.Windows.Forms.Panel();
      this.label1 = new System.Windows.Forms.Label();
      this.AddButton = new System.Windows.Forms.Button();
      this.AddTextBox = new System.Windows.Forms.TextBox();
      this.panel2 = new System.Windows.Forms.Panel();
      this.label3 = new System.Windows.Forms.Label();
      this.FillButton = new System.Windows.Forms.Button();
      this.FillTextBox = new System.Windows.Forms.TextBox();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.AddButton);
      this.panel1.Controls.Add(this.AddTextBox);
      this.panel1.Location = new System.Drawing.Point(12, 12);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(321, 56);
      this.panel1.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(89, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Добавить число";
      // 
      // AddButton
      // 
      this.AddButton.Location = new System.Drawing.Point(222, 20);
      this.AddButton.Name = "AddButton";
      this.AddButton.Size = new System.Drawing.Size(93, 23);
      this.AddButton.TabIndex = 1;
      this.AddButton.Text = "Добавить";
      this.AddButton.UseVisualStyleBackColor = true;
      this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
      // 
      // AddTextBox
      // 
      this.AddTextBox.Location = new System.Drawing.Point(6, 22);
      this.AddTextBox.Name = "AddTextBox";
      this.AddTextBox.Size = new System.Drawing.Size(210, 20);
      this.AddTextBox.TabIndex = 0;
      // 
      // panel2
      // 
      this.panel2.Controls.Add(this.label3);
      this.panel2.Controls.Add(this.FillButton);
      this.panel2.Controls.Add(this.FillTextBox);
      this.panel2.Location = new System.Drawing.Point(12, 74);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(321, 56);
      this.panel2.TabIndex = 3;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 6);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(213, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Заполнить массив случайными числами";
      // 
      // FillButton
      // 
      this.FillButton.Location = new System.Drawing.Point(222, 20);
      this.FillButton.Name = "FillButton";
      this.FillButton.Size = new System.Drawing.Size(93, 23);
      this.FillButton.TabIndex = 1;
      this.FillButton.Text = "Заполнить";
      this.FillButton.UseVisualStyleBackColor = true;
      this.FillButton.Click += new System.EventHandler(this.FillButton_Click);
      // 
      // FillTextBox
      // 
      this.FillTextBox.Location = new System.Drawing.Point(6, 22);
      this.FillTextBox.Name = "FillTextBox";
      this.FillTextBox.Size = new System.Drawing.Size(210, 20);
      this.FillTextBox.TabIndex = 0;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(646, 373);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.panel1);
      this.Name = "Form1";
      this.Text = "Form1";
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button AddButton;
    private System.Windows.Forms.TextBox AddTextBox;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button FillButton;
    private System.Windows.Forms.TextBox FillTextBox;
  }
}

