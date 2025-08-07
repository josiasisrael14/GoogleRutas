using GoogleRuta.ViewModels;

namespace GoogleRuta.Services.Interfaces;

public interface IProjectService
{

   Task Add(ProjectViewModel viewModel);
   Task<ProjectViewModel> GetById(int id);
   Task<List<ProjectViewModel>> GetAll();
   Task<bool>Delete(int id);

}