using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Interfaces
{
    public interface IViewGallerySrvice : IBaseService
    {
        Task GetUserGallery();
        Task InviteUser();
        Task GetInvitations();
        Task AcceptInvitation();
        Task RemoveInvitation();
        Task LikeImage();
        Task AddComment();
    }
}
