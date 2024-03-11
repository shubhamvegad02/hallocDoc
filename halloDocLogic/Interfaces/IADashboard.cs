using halloDocEntities.ViewDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace halloDocLogic.Interfaces
{
    public interface IADashboard
    {
        Task<List<ADashTable>> ADashTableData(int n);

        public ARequestCount ReqCount();

        public AViewNoteCase VCData(int rid);

        AViewNoteCase VNData(int rid, AViewNoteCase vnc);

        Task<int> CancelCase(int rid, ADashTable dt);

        Task AssignCase(int rid, ADashTable dt);

        Task<int> BlockCase(int rid, ADashTable dt);

        Task<bool> VNDatapost(AViewNoteCase vnc);

        Task<bool> VCDataPost(AViewNoteCase vnc);

        Task<int> DeleteFile(string file);

        public string fileNameFromId(int fileId);

        public int ClearCase(int rid);

        public int sendAgreement(int rid);

        public bool sendMail(string email, string? subject, string? message);

        Task<List<ViewUploadedDoc>> closeCase(int rid);

        Task<bool> closeCasePost(int rid, AViewNoteCase vnc);
    }
}
