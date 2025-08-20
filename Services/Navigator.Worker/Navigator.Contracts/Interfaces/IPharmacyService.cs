using Navigator.Contracts.Models;

namespace Navigator.Contracts.Interfaces
{
    public interface IPharmacyService
    {
        void Import(PharmacyImportBO pharmacyImportBO);
    }
}
