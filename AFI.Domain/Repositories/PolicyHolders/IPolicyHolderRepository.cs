using AFI.Domain.Repositories.Base;
using AFI.Domain.Models.PolicyHolders;
using System.Threading.Tasks;

namespace AFI.Domain.Repositories.PolicyHolders
{
    public interface  IPolicyHolderRepository: IBaseRepository<PolicyHolder>
    {
        public Task<PolicyHolder> AddEdit(PolicyHolder PolicyHolder);

    }
}
