/*
 * Copyright 2020 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START kms_verify_asymmetric_signature_rsa]
using System;
using System.Text;
using System.Security.Cryptography;

using Google.Cloud.Kms.V1;

public class VerifyAsymmetricSignatureRsaSample
{

    public bool VerifyAsymmetricSignatureRsa(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key", string keyVersionId = "123",
      string message = "my message",
      byte[] signature = null)
    {
        // Calculate the digest of the message.
        var sha256 = SHA256.Create();
        var digest = sha256.ComputeHash(Encoding.UTF8.GetBytes(message));

        // Get the public key.
        var client = KeyManagementServiceClient.Create();
        var publicKey = client.GetPublicKey(new GetPublicKeyRequest
        {
            CryptoKeyVersionName = new CryptoKeyVersionName(projectId, locationId, keyRingId, keyId, keyVersionId),
        });

        // Split the key into blocks and base64-decode the PEM parts.
        var blocks = publicKey.Pem.Split("-", StringSplitOptions.RemoveEmptyEntries);
        var pem = Convert.FromBase64String(blocks[1]);

        // Create a new RSA key.
        var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(pem, out _);

        // Verify the signature.
        var verified = rsa.VerifyHash(digest, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

        // Return the result.
        return verified;
    }
}
// [END kms_verify_asymmetric_signature_rsa]