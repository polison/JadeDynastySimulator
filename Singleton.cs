using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadeDynastySimulator
{
    public class Singleton<T> where T : Singleton<T>
    {
        /// <summary>
        /// The static reference to the instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncObj)
                    {
                        if (_instance == null)
                        {
                            _instance = Activator.CreateInstance<T>();
                        }
                    }
                }
                return _instance;
            }
            protected set
            {
                _instance = value;
            }
        }
        private static T _instance;
        private static readonly object syncObj = new object();

        /// <summary>
        /// Gets whether an instance of this singleton exists.
        /// </summary>
        public static bool InstanceExists => Instance != null;


        /// <summary>
        /// Gets the instance of this singleton, and returns true if it is not null.
        /// Prefer this whenever you would otherwise use InstanceExists and Instance together.
        /// </summary>
        public static bool TryGetInstance(out T result)
        {
            result = Instance;

            return result != null;
        }
    }
}
