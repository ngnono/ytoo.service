using System;

namespace Intime.OPC.Common.Logger
{
    public interface ILog
    {
        /// <summary>
        ///     ��Ϣ
        /// </summary>
        /// <param name="obj"></param>
        void Info(object obj);

        /// <summary>
        ///     �쳣
        /// </summary>
        /// <param name="obj"></param>
        [Obsolete("��ʹ��Error����")]
        void Exception(object obj);

        /// <summary>
        ///     ������Ϣ
        /// </summary>
        /// <param name="obj"></param>
        void Debug(object obj);

        /// <summary>
        ///     ����
        /// </summary>
        /// <param name="obj"></param>
        void Warn(object obj);

        /// <summary>
        ///     ����
        /// </summary>
        /// <param name="obj"></param>
        void Error(object obj);
    }
}