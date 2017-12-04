/* ==============================================================================
* Description：EarlyWarningResult  
* Author     ：litao
* Create Date：2017/12/4 20:02:17
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    class EarlyWarningResult
    {
        private bool _isIntialized;

        public EarlyWarningSqlDb SqlDb { get; private set; }

        public EarlyWarningSerializer Serializer { get; private set; }

        public EarlyWarningResult()
        {
            SqlDb = new EarlyWarningSqlDb();
            Serializer = new EarlyWarningSerializer();
        }

        public bool Initialize()
        {
            _isIntialized = false;

            _isIntialized =SqlDb.Initialize();
            if(!_isIntialized)
            {
                return _isIntialized;
            }
            Serializer.Initialize();

            _isIntialized = true;
            return _isIntialized;
        }
    }
}
