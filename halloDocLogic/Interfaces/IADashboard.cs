using halloDocEntities.DataModels;
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
        /*Task<List<ADashTable>> ADashTableData(int n);*/

        Task<PagedList<ADashTable>> ADashTableData(int n, int pageNumber = 1);

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

        public bool sendMail(string email, string? subject, string? message, string[]? filenames);

        Task<List<ViewUploadedDoc>> closeCase(int rid);

        Task<int> closeCasePost(int rid, AViewNoteCase vnc);

        Task<int> closeCasefinal(int rid);

        Task<EncounterData> Encounter(int rid, EncounterData ed);

        Task<int> EncounterPost(int rid, EncounterData ed);

        public string EmailFromRid(int rid);

        public string encry(string pass);

        public int AdminidFromAspid(string aspid);

        Task<AdminProfile> MyProfile(int aid);

        public bool ProfilePasswordSubmit(int aid, AdminProfile ap);

        public bool MyProfilePost(int aid, AdminProfile ap);

        Task<List<ViewUploadedDoc>> ViewUpload(int rid);

        public bool OrderPost(int rid, SendOrder so);

        Task<SendOrder> order();

        Task<int> statusFromRid(int rid);

        Task<object> GetOrderData(int businessId);

        Task<bool> TransferCase(int rid, ADashTable adt);

        public List<Physician> getPhysiciandata();

        public List<Region> getRegiondata();

        public List<Request> ReqData();
    }

}
