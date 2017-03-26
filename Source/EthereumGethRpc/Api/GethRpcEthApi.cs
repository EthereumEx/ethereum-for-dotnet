﻿using EthereumGethRpc.DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthereumGethRpc.Api
{
    /// <summary>
    /// Encapsulation of ETH rpc api for Geth
    /// Not intended to be used in a standalone instance (instance created by GethRpcProxy class)
    /// based on https://github.com/ethereum/wiki/wiki/JSON-RPC  and  https://github.com/ethereum/go-ethereum/wiki/Management-APIs
    /// </summary>
    public class GethRpcEthApi : JsonRpcBase
    {
        Uri rpcApiEndpoint = null;
        internal GethRpcEthApi(Uri endpoint) 
            : base (endpoint)
        {
        }

        /// <summary>
        /// return the Ethereum client software version
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetProtocolVersionAsync()
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_protocolVersion\",\"params\":[],\"id\":" + GetNewId().ToString() + "}";
            return await ExecuteRpcRequestAsync(rpcReq);
        }



        /// <summary>
        /// return the the syncing status of the etheruem client
        /// </summary>
        /// <returns>true =currenlty syncing, false not syncing</returns>
        public async Task<SyncingStatus> SyncingStatusAsync()
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_syncing\",\"params\":[],\"id\":" + GetNewId().ToString() + "}";

            var res = await ExecuteRpcRequestAsync(rpcReq);
            //res = "{ startingBlock: '0x384', currentBlock: '0x386', highestBlock: '0x454'}"; // for testing deserialization

            if (res == "false")
                return null;

            // if not "false" -> deserialization to SyncingStatus
            var status = JsonConvert.DeserializeObject<SyncingStatus>(res);
            return status;
        }

        /// <summary>
        /// Return the coin base address (account id use a coin base in the ethereum blockchain)
        /// </summary>
        /// <returns>address of coin base (hexString)</returns>
        public async Task<string> GetCoinbaseAddressAsync()
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_coinbase\",\"params\":[],\"id\":" + GetNewId().ToString() + "}";
            return await ExecuteRpcRequestAsync(rpcReq);
        }

        /// <summary>
        /// return the maning status of Geth.
        /// </summary>
        /// <returns>true=geth is mining, false= geth is not mining</returns>
        public async Task<bool> IsMiningAsync()
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_mining\",\"params\":[],\"id\":" + GetNewId().ToString() + "}";
            return Convert.ToBoolean(await ExecuteRpcRequestAsync(rpcReq));
        }

        public async Task<Int64> GetHashRateAsync()
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_hashrate\",\"params\":[],\"id\":" + GetNewId().ToString() + "}";
            var result = await ExecuteRpcRequestAsync(rpcReq);
            return Int64FromQuantity(result);
        }

        public async Task<Int64> GetGasPriceAsync()
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_gasPrice\",\"params\":[],\"id\":" + GetNewId().ToString() + "}";
            var result = await ExecuteRpcRequestAsync(rpcReq);
            return Int64FromQuantity(result);
        }

        public async Task<string[]> GetAccountsAsync()
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_accounts\",\"params\":[],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<string[]>(rpcReq);
            return res;
        }

        public async Task<Int64> GetMostRecentBlockNumberAsync()
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_blockNumber\",\"params\":[],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync(rpcReq);
            return Int64FromQuantity(res);
        }


        /// <summary>
        /// return the balance of an, account (in wei?)
        /// </summary>
        /// <param name="accountId">account address</param>
        /// <param name="blockNumber">block number, or 'latest' or 'earliest' or 'pending'. Default is 'latest'</param>
        /// <returns>balance (in ether)</returns>
        public async Task<string> GetBalanceAsync(string accountId, string blockNumber = "latest")
        {
            // TODO : add support for int256 ( nuget BigMath ?)
            string rpcReq = BuildRpcRequest("eth_getBalance", accountId, blockNumber);
            var res = await ExecuteRpcRequestAsync(rpcReq);
            return res;
        }


        /// <summary>
        /// NOT TESTED
        /// </summary>
        /// <param name="storageAddress"></param>
        /// <param name="positionInStorage"></param>
        /// <param name="blockNumber">block number, or 'latest' or 'earliest' or 'pending'</param>
        /// <returns></returns>
        public async Task<string> GetStorageAtAsync(string storageAddress, string positionInStorage, string blockNumber = "latest")
        {
            // TODO : add support for int256 ( nuget BigMath ?)
            // TODO TEST
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getStorageAt\",\"params\":[\"" + storageAddress + "\", \"" + positionInStorage + "\", \"" + blockNumber + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync(rpcReq);
            return res;
        }

        public async Task<string> GetTransactionCountAsync(string address, string blockNumber = "latest")
        {
            // TODO : add support for int128 ( nuget BigMath)
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getTransactionCount\",\"params\":[\"" + address + "\",\"" + blockNumber + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// </summary>
        /// <param name="blockHash"></param>
        /// <returns></returns>
        public async Task<string> GetBlockTransactionCountByHashAsync(string blockHash)
        {
            // TODO : to test 
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getBlockTransactionCountByHash\",\"params\":[\"" + blockHash + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// </summary>
        /// <param name="blockNumber">block number, or 'latest' or 'earliest' or 'pending'</param>
        /// <returns></returns>
        public async Task<string> GetBlockTransactionCountByNumberAsync(string blockNumber = "latest")
        {
            // TODO : to test 
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getBlockTransactionCountByNumber\",\"params\":[\"" + blockNumber + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// </summary>
        /// <param name="blockHash"></param>
        /// <returns></returns>
        public async Task<string> GetUncleCountByBlockHashAsync(string blockHash)
        {
            // TODO : to test 
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getUncleCountByBlockHash\",\"params\":[\"" + blockHash + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// </summary>
        /// <param name="blockNumber">block number, or 'latest' or 'earliest' or 'pending'</param>
        /// <returns></returns>
        public async Task<string> GetUncleCountByBlockNumberAsync(string blockNumber = "latest")
        {
            // TODO : to test 
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getUncleCountByBlockNumber\",\"params\":[\"" + blockNumber + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// </summary>
        /// <param name="address"></param>
        /// <param name="blockNumber">block number, or 'latest' or 'earliest' or 'pending'</param>
        /// <returns></returns>
        public async Task<string> GetCodeAsync(string address, string blockNumber = "lastest")
        {
            // TODO : to test 
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getCode\",\"params\":[\"" + address + "\",\"" + blockNumber + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// the specified address account must be unlocked.
        /// </summary>
        /// <param name="address">account address/id to used for signing. </param>
        /// <param name="hexDataToSign">bytebuffer formatted as hexa (0xaabbccddeeff001122 ... ) to sign</param>
        /// <returns></returns>
        public async Task<string> SignAsync(string address, string hexDataToSign)
        {
            // TODO : to test 
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_sign\",\"params\":[\"" + address + "\",\"" + hexDataToSign + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync(rpcReq);
            return res;
        }


        /// <summary>
        /// Generate an new transaction in the blockchain.
        /// </summary>
        /// <param name="trx">Transaction ID (used to request receipt after mining)</param>
        /// <returns></returns>
        public async Task<string> SendTransactionAsync(Transaction trx)
        {
            if (string.IsNullOrEmpty(trx?.From))
                throw new NullReferenceException("'fromAddress' parameter can not be null or empty");

            string rpcReq = BuildRpcRequest("eth_sendTransaction", trx);
            var res = await ExecuteRpcRequestAsync(rpcReq);
            return res;
        }

        /// <summary>
        /// Return the transaction receipt using the transaction ID.
        /// </summary>
        /// <param name="trxId">transactionID of the transaction</param>
        /// <returns>null if no receipt available, instance of TransactionReceipt if available</returns>
        public async Task<TransactionReceipt> GetTransactionReceiptAsync(string trxId)
        {
            string rpcReq = BuildRpcRequest("eth_getTransactionReceipt", trxId);
            var res = await ExecuteRpcRequestAsync<TransactionReceipt>(rpcReq);
            return res;
        }

        /// <summary>
        /// Creates new message call transaction or a contract creation for signed transactions.
        /// NOT TESTED
        /// </summary>
        /// <param name="signedTransactionData"></param>
        /// <returns></returns>
        public async Task<string> SendRawTransactionAsync(string signedTransactionData)
        {
            // TODO TEST
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_sendRawTransaction\",\"params\":[\"" + signedTransactionData + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// Executes a new message call immediately without creating a transaction on the block chain.
        /// </summary>
        /// <param name="toAddress">The address the transaction is directed to</param>
        /// <param name="blockNumber"></param>
        /// <param name="data">Hash of the method signature and encoded parameters.</param>
        /// <param name="fromAddress">The address the transaction is sent from</param>
        /// <param name="value">Integer of the value send with this transaction</param>
        /// <param name="gas">Integer of the gas provided for the transaction execution. eth_call consumes zero gas, but this parameter may be needed by some executions</param>
        /// <param name="gasPrice">Integer of the gasPrice used for each paid gas</param>
        /// <returns></returns>
        public async Task<string> CallAsync(string toAddress, string blockNumber = "latest", string data = null, string fromAddress = null, string value = null, string gas = null, string gasPrice = null)
        {
            // TODO : to test 

            if (string.IsNullOrEmpty(toAddress))
                throw new NullReferenceException("'toAddress' parameter can not be null or empty");
            if (string.IsNullOrEmpty(blockNumber))
                throw new NullReferenceException("'blockNumber' parameter can not be null or empty");

            StringBuilder sbTxCallJson = new StringBuilder();
            if (!string.IsNullOrEmpty(fromAddress))
                sbTxCallJson.Append($"{{ \"from\" : \"{fromAddress}\" ");
            sbTxCallJson.Append($", \"to\" : \"{toAddress}\" ");
            if (!string.IsNullOrEmpty(gas))
                sbTxCallJson.Append($", \"gas\" : \"{gas}\" ");
            if (!string.IsNullOrEmpty(gasPrice))
                sbTxCallJson.Append($", \"gasPrice\" : \"{gasPrice}\" ");
            if (!string.IsNullOrEmpty(value))
                sbTxCallJson.Append($", \"value\" : \"{value}\" ");
            if (!string.IsNullOrEmpty(data))
                sbTxCallJson.Append($", \"data\" : \"{data}\" ");
            sbTxCallJson.Append("} ");

            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_call\",\"params\":[\"" + sbTxCallJson.ToString() + "\" , \"" + blockNumber + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// </summary>
        /// <param name="toAddress"></param>
        /// <param name="data"></param>
        /// <param name="fromAddress"></param>
        /// <param name="value"></param>
        /// <param name="gas"></param>
        /// <param name="gasPrice"></param>
        /// <returns></returns>
        public async Task<string> EstimateGasAsync(string toAddress = null, string data = null, string fromAddress = null, string value = null, string gas = null, string gasPrice = null)
        {
            // TODO : to test 

            StringBuilder sbTxCallJson = new StringBuilder();
            if (!string.IsNullOrEmpty(fromAddress))
                sbTxCallJson.Append($", \"from\" : \"{fromAddress}\" ");
            if (!string.IsNullOrEmpty(toAddress))
                sbTxCallJson.Append($", \"to\" : \"{toAddress}\" ");
            if (!string.IsNullOrEmpty(gas))
                sbTxCallJson.Append($", \"gas\" : \"{gas}\" ");
            if (!string.IsNullOrEmpty(gasPrice))
                sbTxCallJson.Append($", \"gasPrice\" : \"{gasPrice}\" ");
            if (!string.IsNullOrEmpty(value))
                sbTxCallJson.Append($", \"value\" : \"{value}\" ");
            if (!string.IsNullOrEmpty(data))
                sbTxCallJson.Append($", \"data\" : \"{data}\" ");
            string json = "{ ";
            if (sbTxCallJson.Length > 0)
            {
                json += sbTxCallJson.ToString().Substring(1); // jump the first ','
            }
            json += "} ";

            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_estimateGas\",\"params\":[ " + json + "],\"id\":" + GetNewId().ToString() + "}";

            var res = await ExecuteRpcRequestAsync(rpcReq);
            return res;
        }

        /// <summary>
        /// Returns information about a block by hash.
        /// Detailed transaction NOT TESTED/NOT WORKING (throw NotImplementedException)
        /// </summary>
        /// <param name="blockHash">Hash of a block</param>
        /// <param name="returnDetailedTransaction">If true it returns the full transaction objects, if false only the hashes of the transactions.</param>
        /// <returns></returns>
        public async Task<Block> GetBlockByHashAsync(string blockHash, bool returnDetailedTransaction = false)
        {
            if (returnDetailedTransaction)
                throw new NotImplementedException("detail transaction not implemented");
            // Detailed transaction not tested

            // TODO : add correc mapping for details result

            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getBlockByHash\",\"params\":[\"" + blockHash + "\" , " + returnDetailedTransaction.ToString().ToLower() + " ],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<Block>(rpcReq);
            return res;
        }

        /// <summary>
        /// Returns information about a block by block number.
        /// Detailed transaction NOT TESTED/NOT WORKING (throw NotImplementedException)
        /// </summary>
        /// <param name="blockNumber"> integer of a block number, or the string "earliest", "latest" or "pending"</param>
        /// <param name="returnDetailedTransaction">If true it returns the full transaction objects, if false only the hashes of the transactions.</param>
        /// <returns></returns>
        public async Task<Block> GetBlockByNumberAsync(string blockNumber = "latest", bool returnDetailedTransaction = false)
        {
            if (returnDetailedTransaction)
                throw new NotImplementedException("detail transaction not implemented");

            // TODO : add correc mapping for details result

            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getBlockByNumber\",\"params\":[\"" + blockNumber + "\" , " + (returnDetailedTransaction ? "true" : "false") + " ],\"id\":" + GetNewId().ToString() + "}";
            return await ExecuteRpcRequestAsync<Block>(rpcReq);
        }

        /// <summary>
        /// Returns the information about a transaction requested by transaction hash
        /// </summary>
        /// <param name="transactionHash">hash of the transaction</param>
        /// <returns>Transacation object if found, null if not found</returns>
        public async Task<Transaction> GetTransactionByHashAsync(string transactionHash)
        {
            //string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getTransactionByHash\",\"params\":[\"" + transactionHash + "\" ],\"id\":" + GetNewId().ToString() + "}";
            string rpcReq = BuildRpcRequest("eth_getTransactionByHash", transactionHash);

            return await ExecuteRpcRequestAsync<Transaction>(rpcReq);
        }

        /// <summary>
        /// Returns information about a transaction by block hash and transaction index position.
        /// </summary>
        /// <param name="blockHash">hash of a block.</param>
        /// <param name="transactionIndex">transaction index position</param>
        /// <returns></returns>
        public async Task<Transaction> GetTransactionByBlockHashAndIndexAsync(string blockHash, string transactionIndex)
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getTransactionByBlockHashAndIndex\",\"params\":[\"" + blockHash + "\" , \"" + transactionIndex + "\" ],\"id\":" + GetNewId().ToString() + "}";
            return await ExecuteRpcRequestAsync<Transaction>(rpcReq);
        }


        /// <summary>
        /// Returns information about a transaction by block number and transaction index position.
        /// </summary>
        /// <param name="blockNumber"> a block number, or the string "earliest", "latest" or "pending"</param>
        /// <param name="transactionIndex">transaction index position.</param>
        /// <returns></returns>
        public async Task<Transaction> GetTransactionByBlockNumberAndIndexAsync(string blockNumber, string transactionIndex)
        {
            string rpcReq = BuildRpcRequest("eth_getTransactionByBlockNumberAndIndex", blockNumber, transactionIndex);
            return await ExecuteRpcRequestAsync<Transaction>(rpcReq);
        }

        /// <summary>
        /// NOT TESTED
        /// Returns information about a uncle of a block by hash and uncle index position.
        /// </summary>
        /// <param name="blockHash">hash a block</param>
        /// <param name="uncleIndex">uncle's index position</param>
        /// <returns></returns>
        public async Task<Block> GetUncleByBlockHashAndIndexAsync(string blockHash, string uncleIndex)
        {
            // TODO TO TEST 

            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getUncleByBlockHashAndIndex\",\"params\":[\"" + blockHash + "\" , \"" + uncleIndex + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<Block>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// Returns information about a uncle of a block by number and uncle index position
        /// </summary>
        /// <param name="blockNumber">a block number, or the string "earliest", "latest" or "pending"</param>
        /// <param name="uncleIndex">uncle's index position</param>
        /// <returns></returns>
        public async Task<Block> GetUncleByBlockNumberAndIndexAsync(string blockNumber = "lastest", string uncleIndex = "0x0")
        {
            // TODO TO TEST 

            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getUncleByBlockNumberAndIndex\",\"params\":[\"" + blockNumber + "\" , \"" + uncleIndex + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<Block>(rpcReq);
            return res;
        }

        /// <summary>
        /// Returns a list of available compilers in the client
        /// </summary>
        /// <returns>Array of available compilers</returns>
        public async Task<string[]> GetCompilersAsync()
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getCompilers\",\"params\":[\"" + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<string[]>(rpcReq);
            return res;
        }

        /// <summary>
        /// Compile a solidity contract source code. 
        /// Solc compiler must be in the PATH of geth.
        /// </summary>
        /// <param name="sourceCode">the source code</param>
        /// <returns>ABI oboject</returns>
        public async Task<Abi> CompileSolidityAsync(string sourceCode)
        {
            var escaped = sourceCode.Replace("\"", @"\""");
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_compileSolidity\",\"params\":[\"" + escaped + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<Abi>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED 
        /// Returns compiled LLL code
        /// </summary>
        /// <param name="sourceCode">The source code.</param>
        /// <returns>compiled source code</returns>
        public async Task<string> CompileLLLAsync(string sourceCode)
        {
            var escaped = sourceCode.Replace("\"", @"\""");
            // TODO : test with solidity & others compilers installed  
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_compileLLL\",\"params\":[\"" + escaped + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<string>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED 
        /// Returns compiled serpent code
        /// </summary>
        /// <param name="sourceCode">serpent source code</param>
        /// <returns>compiled sourcecode</returns>
        public async Task<string> CompileSerpentAsync(string sourceCode)
        {
            // TODO : escape the sourcecode (for quote & double quote)
            // TODO : test with serpent compilers & others compilers installed  
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_compileSerpent\",\"params\":[\"" + sourceCode + "\"],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<string>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT IMPLEMENTED / NOT TESTED
        /// </summary>
        /// <returns></returns>
        public async Task<string> NewFilterAsync()
        {
            throw new NotImplementedException("Eth_NewFilter() NOT IMPLEMENTED");
            return "";
        }

        /// <summary>
        /// NOT TESTED
        /// Creates a filter in the node, to notify when a new block arrives.
        /// </summary>
        /// <returns>filter ID</returns>
        public async Task<string> NewBlockFilterAsync()
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_newBlockFilter\",\"params\":[],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<string>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// Creates a filter in the node, to notify when new pending transactions arrive
        /// </summary>
        /// <returns>filter ID</returns>
        public async Task<string> NewPendingTransactionFilterAsync()
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_newPendingTransactionFilter\",\"params\":[],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<string>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// Uninstalls a filter with given id. Should always be called when watch is no longer needed. 
        /// </summary>
        /// <param name="filterId">Id of filter to desinstall</param>
        /// <returns>true for success, false for failure</returns>
        public async Task<bool> UninstallFilterAsync(string filterId)
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_uninstallFilter\",\"params\":[ \"" + filterId + "\"  ],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<bool>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT FULLY IMPLEMENTED / NOT TESTED
        /// Polling method for a filter, which returns an array of logs which occurred since last poll
        /// </summary>
        /// <param name="filterId">fileter ID</param>
        /// <returns>array of changes from filter - NOT FULLY IMPLEMENTED</returns>
        public async Task<string[]> GetFilterChangesAsync(string filterId)
        {
            // TODO : test with full data return
            //https://github.com/ethereum/wiki/wiki/JSON-RPC#eth_getfilterchanges

            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getFilterChanges\",\"params\":[ \"" + filterId + "\"  ],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<string[]>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT FULLY IMPLEMENTED / NOT TESTED
        /// Returns an array of all logs matching filter with given id.
        /// </summary>
        /// <param name="filterId">filter ID</param>
        /// <returns>array of logs from filter - NOT FULLY IMPLEMENTED</returns>
        public async Task<string[]> GetFilterLogsAsync(string filterId)
        {
            // TODO : test with full data return
            //https://github.com/ethereum/wiki/wiki/JSON-RPC#eth_getfilterlogs

            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getFilterLogs\",\"params\":[ \"" + filterId + "\"  ],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<string[]>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT FULLY IMPLEMENTED / NOT TESTED
        /// Returns an array of all logs matching a given filter object.
        /// </summary>
        /// <param name="topics">list of topics</param>
        /// <returns>logs matching topics - NOT FULLY IMPLEMENTED</returns>
        public async Task<string[]> GetLogsAsync(params string[] topics)
        {
            // TODO : test with full data return
            //https://github.com/ethereum/wiki/wiki/JSON-RPC#eth_getlogs

            var topicsJson = JsonConvert.SerializeObject(topics);
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getLogs\",\"params\":[ { \"topics\" : " + topicsJson + "}  ],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<string[]>(rpcReq);
            // TODO : IMPLEMENT CUSTOM TYPE FOR RESULT

            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// Returns the hash of the current block, the seedHash, and the boundary condition to be met ("target").
        /// </summary>
        /// <returns>[0]:current block header pow-hash,
        /// [1]:the seed hash used for the DAG
        /// [2]:the boundary condition ("target"), 2^256 / difficulty</returns>
        public async Task<string[]> GetWorkAsync()
        {
            // TODO : finalize test with mining instance of Geth
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getWork\",\"params\":[],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<string[]>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// Used for submitting a proof-of-work solution
        /// </summary>
        /// <param name="nonce">The nonce found (64 bits)</param>
        /// <param name="powhash">The header's pow-hash (256 bits)</param>
        /// <param name="mixdigest">The mix digest (256 bits)</param>
        /// <returns>true if the provided solution is valid, otherwise false</returns>
        public async Task<bool> SubmitWorkAsync(string nonce, string powhash, string mixdigest)
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_submitWork\",\"params\":[ \"" + nonce + "\", \"" + powhash + "\",\"" + mixdigest + "\" ],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<bool>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// Used for submitting mining hashrate.
        /// </summary>
        /// <param name="hashrate">hexadecimal string representation (32 bytes) of the hash rate</param>
        /// <param name="id">random hexadecimal(32 bytes) ID identifying the client</param>
        /// <returns>true if submitting went through succesfully and false otherwise.</returns>
        public async Task<bool> SubmitHashRateAsync(string hashrate, string id)
        {

            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_submitHashrate\",\"params\":[ \"" + hashrate + "\", \"" + id + "\" ],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<bool>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED 
        /// Stores a string in the local database.
        /// </summary>
        /// <param name="databaseName">Database name.</param>
        /// <param name="keyName">Key name.</param>
        /// <param name="value">String to store</param>
        /// <returns>true if the value was stored, otherwise false.</returns>
        public async Task<bool> PutStringAsync(string databaseName, string keyName, string value)
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_putString\",\"params\":[ \"" + databaseName + "\", \"" + keyName + "\",\"" + value + "\" ],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<bool>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED
        /// Returns string from the local database.
        /// </summary>
        /// <param name="databaseName">Database name.</param>
        /// <param name="keyName">Key name.</param>
        /// <returns>the retrieved string</returns>
        public async Task<string> GetStringAsync(string databaseName, string keyName)
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getString\",\"params\":[ \"" + databaseName + "\", \"" + keyName + "\" ],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<string>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED 
        /// Stores a hex in the local database.
        /// </summary>
        /// <param name="databaseName">Database name.</param>
        /// <param name="keyName">Key name.</param>
        /// <param name="hexvalue">hexa value to store</param>
        /// <returns>true if the value was stored, otherwise false.</returns>
        public async Task<bool> PutHexAsync(string databaseName, string keyName, string hexvalue)
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_putHex\",\"params\":[ \"" + databaseName + "\", \"" + keyName + "\",\"" + hexvalue + "\" ],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<bool>(rpcReq);
            return res;
        }

        /// <summary>
        /// NOT TESTED 
        /// Returns hexa from the local database.
        /// </summary>
        /// <param name="databaseName">Database name.</param>
        /// <param name="keyName">Key name.</param>
        /// <returns>the retrieved hex</returns>
        public async Task<string> GetHexAsync(string databaseName, string keyName)
        {
            string rpcReq = "{ \"jsonrpc\":\"2.0\",\"method\":\"eth_getHex\",\"params\":[ \"" + databaseName + "\", \"" + keyName + "\" ],\"id\":" + GetNewId().ToString() + "}";
            var res = await ExecuteRpcRequestAsync<string>(rpcReq);
            return res;
        }
    }
}