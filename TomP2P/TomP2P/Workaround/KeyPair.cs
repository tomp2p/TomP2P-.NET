﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomP2P.Workaround
{
    /// <summary>
    /// As there is no .NET equivalent to the java.security.KeyPair class,
    /// this class serves as a workaround.
    /// </summary>
    public class KeyPair
    {
        public KeyPair(IPublicKey publicKey, IPrivateKey privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        public IPublicKey PublicKey { get; private set; }

        public IPrivateKey PrivateKey { get; private set; }
    }
}
