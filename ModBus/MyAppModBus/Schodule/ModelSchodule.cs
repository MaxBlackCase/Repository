using System.Security.Cryptography.X509Certificates;

namespace MyAppModBus.Schodule {
  public class ModelSchodule {

    public string Name { get; set; }

    public string SchGetGraph() {

      Name = "Get Out";
      return Name;
    }
  }
}
