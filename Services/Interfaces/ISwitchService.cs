using GoogleRuta.Models;
namespace GoogleRuta.Services.Interfaces
{
    public interface ISwitchService
    {
        Task<Switchs> Add(Switchs switchs);
        Task<Switchs> Update(Switchs switchs);
        Task<Switchs> GetById(int id);
        Task<IEnumerable<Switchs>> GetAll();
        Task<bool> Delete(int id);
    }
}