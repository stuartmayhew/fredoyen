using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FredOyen.Models;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;

namespace FredOyen.Controllers
{
    public class HomeController : Controller
    {
        clsDataGetter dg = new clsDataGetter(ConfigurationManager.ConnectionStrings["FredOyen"].ConnectionString);
        FredOyenData db = new FredOyenData();
        private const string TempPath = @"C:\Temp";
        public ActionResult Index(int? userID)
        {
            bool isPaul = true;
            bool isAA = true;
            IEnumerable<Talks> talks = null;
            if (userID != null)
            {
                SqlDataReader dr = dg.GetDataReader("SELECT * FROM Emails WHERE pKey=" + userID.ToString());
                if (dr.Read())
                {
                    isPaul = (bool)dr["isPaul"];
                    isAA = (bool)dr["isSpeaker"];
                }
                dg.KillReader(dr);
                if (isPaul && isAA)
                {
                    talks = db.Talks.ToList().OrderByDescending(x => x.talk_id);
                    return View("Index", talks);
                }
                else if (isPaul && !isAA)
                {
                    talks = db.Talks.Where(x => x.isPaul == true).ToList().OrderByDescending(x => x.talk_id);
                    return View("Index", talks);
                }

                else if (!isPaul && isAA)
                {
                    talks = db.Talks.Where(x => x.isPaul == false).ToList().OrderByDescending(x => x.talk_id);
                    return View("Index", talks);
                }
            }
            talks = db.Talks.ToList().OrderByDescending(x => x.talk_id);
            return View("Index", talks);
        }

        public ActionResult UploadFile(int TalkID)
        {
            Talks talk = new Talks();
            talk.sourceFile = dg.GetScalarString("SELECT talkPath FROM TalkFiles WHERE fileID=" + TalkID.ToString());
            talk.SentDate = DateTime.Now;
            return View("FileUpload", talk);
        }

        public void TestUpload()
        {
            Talks talk = new Talks();
            talk.sourceFile = @"D:\AATalks\AASpeakers\AA\AASpeakersAAFredOCAUnity03.mp3";

            FileUpload(talk);
        }
        public ActionResult FileUpload(Talks talk)
        {
            System.IO.FileInfo fi = new FileInfo(talk.sourceFile);
            talk.fileBytes = dg.GetScalaBytes("SELECT fileBytes FROM TalkFiles WHERE talkPath='" + talk.sourceFile + "'");
            var ext = fi.Extension;
            var fileName = fi.Name;
            fileName = fileName.Replace("-", "");
            fileName = fileName.Replace(" ", "");
            var basePath = Server.MapPath("~/Speakers");
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            var docPath = Path.Combine(basePath, fileName);
            string docURL = "http://fredoyen.org/Speakers/" + fileName;
            ViewBag.docURL = docURL;

            try
            {
                System.IO.File.WriteAllBytes(docPath, talk.fileBytes);
                talk.filePath = docURL;
                ViewBag.Success = true;
            }
            catch (Exception ex)
            {
                ViewBag.Success = false;
                ViewBag.Error = ex.Message;
            }
            SaveTalk(talk);
            return View("FileUpload", talk);
        }

        private void SaveTalk(Talks talk)
        {
            talk.SentDate = DateTime.Now;

            string sql = "INSERT INTO Talks(isPaul,filePath,Comment,SentDate) VALUES(";
            sql += dg.ConvertBool((bool)talk.isPaul) + ",";
            sql += "'" + talk.filePath + "',";
            sql += "'" + dg.FSQ(talk.Comment) + "',";
            sql += "'" + talk.SentDate.ToShortDateString() + "')";
            dg.RunCommand(sql);

        }
    }
}