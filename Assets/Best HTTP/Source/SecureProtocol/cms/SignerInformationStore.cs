#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System.Collections;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Cms
{
    public class SignerInformationStore
    {
        private readonly IList all; //ArrayList[SignerInformation]
        private readonly IDictionary table = BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateHashtable(); // Hashtable[SignerID, ArrayList[SignerInformation]]

        /**
         * Create a store containing a single SignerInformation object.
         *
         * @param signerInfo the signer information to contain.
         */
        public SignerInformationStore(
            SignerInformation signerInfo)
        {
            this.all = BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateArrayList(1);
            this.all.Add(signerInfo);

            SignerID sid = signerInfo.SignerID;

            table[sid] = all;
        }

        /**
         * Create a store containing a collection of SignerInformation objects.
         *
         * @param signerInfos a collection signer information objects to contain.
         */
        public SignerInformationStore(
            ICollection signerInfos)
        {
            foreach (SignerInformation signer in signerInfos)
            {
                SignerID sid = signer.SignerID;
                IList list = (IList)table[sid];

                if (list == null)
                {
                    table[sid] = list = BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateArrayList(1);
                }

                list.Add(signer);
            }

            this.all = BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateArrayList(signerInfos);
        }

        /**
        * Return the first SignerInformation object that matches the
        * passed in selector. Null if there are no matches.
        *
        * @param selector to identify a signer
        * @return a single SignerInformation object. Null if none matches.
        */
        public SignerInformation GetFirstSigner(
            SignerID selector)
        {
            IList list = (IList) table[selector];

            return list == null ? null : (SignerInformation) list[0];
        }

        /// <summary>The number of signers in the collection.</summary>
        public int Count
        {
            get { return all.Count; }
        }

        /// <returns>An ICollection of all signers in the collection</returns>
        public ICollection GetSigners()
        {
            return BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateArrayList(all);
        }

        /**
        * Return possible empty collection with signers matching the passed in SignerID
        *
        * @param selector a signer id to select against.
        * @return a collection of SignerInformation objects.
        */
        public ICollection GetSigners(
            SignerID selector)
        {
            IList list = (IList) table[selector];

            return list == null ? BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateArrayList() : BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateArrayList(list);
        }
    }
}
#pragma warning restore
#endif
