using System;
using System.ComponentModel;
using test_wpf.Models;

namespace test_wpf
{
  internal class FileIOService
  {
    private string pATH;

    public FileIOService(string pATH)
    {
      this.pATH = pATH;
    }

    internal BindingList<TodoModel> LoadData()
    {
      throw new NotImplementedException();
    }

    internal void SaveData(object sender)
    {
      throw new NotImplementedException();
    }
  }
}