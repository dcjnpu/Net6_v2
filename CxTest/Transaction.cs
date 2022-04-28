using System.Transactions;

namespace CxTest
{
    /// <summary>
    /// 关系数据库事务写法
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// 事务写法
        /// </summary>
        public void TestTransaction()
        {
            using TransactionScope transactionScope = new TransactionScope();
            using TransactionScope transactionScope_child = new TransactionScope(TransactionScopeOption.Required);

            //do something


            transactionScope_child.Complete();
            transactionScope.Complete();
        }

        public async Task TestTransactionAsync()
        {
            using TransactionScope transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            using TransactionScope transactionScope_child = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);

            await DoTask();


            transactionScope_child.Complete();
            transactionScope.Complete();
            //return Task.CompletedTask;
        }

        public static Task DoTask()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("TestTransactionAsync");
            });
        }
    }
}