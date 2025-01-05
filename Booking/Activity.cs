namespace Eksamens_Projekt_SystemIntegration_Jens_Ulrik
{
    public class Activity
    {
        public string Name { get; set; }
        public double MedlemsPris { get; set; }
        public double IkkeMedlemsPris { get; set; }

        public Activity(string name, double medlemsPris, double ikkeMedlemsPris)
        {
            Name = name;
            MedlemsPris = medlemsPris;
            IkkeMedlemsPris = ikkeMedlemsPris;
        }
    }
}