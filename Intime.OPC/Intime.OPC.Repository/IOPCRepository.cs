using System.Collections.Generic;

namespace Intime.OPC.Repository
{
    public interface IOPCRepository<in TKey, TEntity> where TEntity : class
    {
        /// <summary>
        /// ����IDɾ��
        /// </summary>
        /// <param name="id">����ID��ɾ��</param>
        void Delete(TKey id);

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="entity">Ҫ���µ�ʵ��</param>
        void Update(TEntity entity);

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="entity">Ҫ�����ʵ��</param>
        /// <returns name="T">���ظ��º��ʵ��</returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="entity">Ҫ�����ʵ��</param>
        /// <returns name="T">���ظ��º��ʵ��</returns>
        void InsertOrUpdate(List<TEntity> entity);

        /// <summary>
        /// ȫ��
        /// </summary>
        /// <returns></returns>
        List<TEntity> FindAll();

        /// <summary>
        /// ����key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TEntity GetItem(TKey key);

        /// <summary>
        /// Autocomplete interface
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IEnumerable<TEntity> AutoComplete(string query);
    }
}