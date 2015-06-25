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

namespace LittleFlame
{
    public class SaveData
    {
        StorageDevice device;
        string containerName = "saveData";
        string filename;

        Vector3 playerPosition;
        public Vector3 PlayerPosition
        {
            get { return playerPosition; }
        }

        float playerSize;
        public float PlayerSize
        {
            get { return playerSize; }
        }

        VertexMultitextured[] terrain;
        public VertexMultitextured[] Terrain
        {
            get { return terrain; }
        }

        Vector3[] modelRotation;
        public Vector3[] ModelRotation
        {
            get { return modelRotation; }
        }

        bool[] modelActivated;
        public bool[] ModelActivated
        {
            get { return modelActivated; }
        }

        public struct SaveGame
        {
            public Vector3 PlayerPosition;
            public float PlayerSize;
            public VertexMultitextured[] Terrain;
            public Vector3[] ModelRotation;
            public bool[] ModelActivated;
        }

        public void InitiateSave(Game1 game, Vector3 playerPosition, float playerSize, VertexMultitextured[] terrain, Vector3[] modelRotation, bool[] modelActivated, string filename)
        {
            this.playerPosition = playerPosition;
            this.playerSize = playerSize;
            this.terrain = terrain;
            this.modelRotation = modelRotation;
            this.modelActivated = modelActivated;

#if WINDOWS
            FileStream fs = new FileStream("lastsave.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine(filename);
            Console.WriteLine("Last save created with name: " + filename);

            sw.Close();
            fs.Close();
#endif

#if XBOX
            Save.GlobalData gd = new Save.GlobalData();
            gd.InitiateSave(game, filename, "global");
#endif

            this.filename = filename + ".sav";
            try
            {
                device = null;
                StorageDevice.BeginShowSelector(SaveToDevice, null);
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

                SaveGame data = new SaveGame()
                {
                    PlayerPosition = playerPosition,
                    PlayerSize = playerSize,
                    Terrain = terrain,
                    ModelRotation = modelRotation,
                    ModelActivated = modelActivated,
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

                XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));

                serializer.Serialize(stream, data);

                stream.Close();

                container.Dispose();
            }

            catch (System.ArgumentException e) {
                Console.WriteLine(e.Message);
            }
        }

        public void InitiateLoad(Game1 game, string filename)
        {
            this.filename = filename + ".sav";

            try
            {
                device = null;
                StorageDevice.BeginShowSelector(LoadFromDevice, null);
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

                XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));

                SaveGame data = (SaveGame)serializer.Deserialize(stream);

                stream.Close();

                container.Dispose();

                //Update the game based on the save game file
                playerPosition = data.PlayerPosition;
                playerSize = data.PlayerSize;
                terrain = data.Terrain;
                modelRotation = data.ModelRotation;
                modelActivated = data.ModelActivated;
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
