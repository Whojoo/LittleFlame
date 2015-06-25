using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.GamerServices;

namespace LittleFlame.Save
{
    public class GlobalData
    {
        StorageDevice device;
        string containerName = "saveData";
        string filename;

        string lastsave;
        public string Lastsave
        {
            get { return lastsave; }
        }

        public struct Global
        {
            public string Lastsave;
        }

        public void InitiateSave(Game1 game, string lastsave, string filename)
        {
            this.lastsave = lastsave;

            this.filename = filename + ".sav";
            
            try
            {
                if (!Guide.IsVisible)
                {
                    device = null;
                    StorageDevice.BeginShowSelector(SaveToDevice, null);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void SaveToDevice(IAsyncResult r)
        {
            try
            {
                device = StorageDevice.EndShowSelector(r);

                Global data = new Global()
                {
                    Lastsave = lastsave,
                };

                IAsyncResult result = device.BeginOpenContainer(containerName, null, null);

                result.AsyncWaitHandle.WaitOne();

                StorageContainer container = device.EndOpenContainer(result);

                result.AsyncWaitHandle.Close();

                if (container.FileExists(this.filename))
                {
                    container.DeleteFile(this.filename);
                }

                Stream stream = container.CreateFile(this.filename);

                XmlSerializer serializer = new XmlSerializer(typeof(Global));

                serializer.Serialize(stream, data);

                stream.Close();

                container.Dispose();
            }

            catch (System.ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void InitiateLoad(Game1 game, string filename)
        {
            this.filename = filename + ".sav";

            try
            {
                if (!Guide.IsVisible)
                {
                    device = null;
                    StorageDevice.BeginShowSelector(LoadFromDevice, null);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        void LoadFromDevice(IAsyncResult r)
        {
            try
            {
                device = StorageDevice.EndShowSelector(r);

                IAsyncResult result = device.BeginOpenContainer(containerName, null, null);

                result.AsyncWaitHandle.WaitOne();

                StorageContainer container = device.EndOpenContainer(result);

                result.AsyncWaitHandle.Close();

                if (!container.FileExists(this.filename))
                {
                    container.Dispose();

                    return;
                }

                Stream stream = container.OpenFile(this.filename, FileMode.Open);

                XmlSerializer serializer = new XmlSerializer(typeof(Global));

                Global data = (Global)serializer.Deserialize(stream);

                stream.Close();

                container.Dispose();

                //Update the game based on the save game file
                lastsave = data.Lastsave;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
