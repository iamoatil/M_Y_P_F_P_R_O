/* ==============================================================================
* Description：ExtactionCategory 
*           此文件中定义了4个关于Category的类，他们都继承于AbstractCategory类
*           以下左边的是右边的集合类           
*               ExtactionCategoryCollectionManager -------》ExtactionCategoryCollection
*               ExtactionCategoryCollection        -------》 ExtactionCategory
*               ExtactionCategory                  -------》 ExtactionSubCategory
*               ExtactionSubCategory               -------》 ExtactionItem
*           
* Author     ：litao
* Create Date：2017/11/24 15:34:06
* ==============================================================================*/


namespace XLY.SF.Project.EarlyWarningView
{
    /* ==============================================================================
*           此文件中定义了4个关于Category的类，他们都继承于AbstractCategory类
*           以下左边的是右边的集合类           
*               ExtactionCategoryCollectionManager -------》ExtactionCategoryCollection
*               ExtactionCategoryCollection        -------》 ExtactionCategory
*               ExtactionCategory                  -------》 ExtactionSubCategory
*               ExtactionSubCategory               -------》 ExtactionItem
*           

* ==============================================================================*/

    class ExtactionCategoryCollectionManager : AbstractCategory
    {
        protected override void Add(string name)
        {
            Children.Add(name, new ExtactionCategoryCollection() { Name = name });
        }
    }

    class ExtactionCategoryCollection : AbstractCategory
    {
        protected override void Add(string name)
        {
            Children.Add(name, new ExtactionCategory() { Name = name });
        }
    }

    class ExtactionCategory : AbstractCategory
    {
        protected override void Add(string name)
        {
            Children.Add(name, new ExtactionSubCategory() { Name = name });
        }
    }

    /// <summary>
    /// 此为核心。其下是数据，比如它的名字为“安装应用（10）”，其下的数据就为安装了的APP，比如QQ，微信等
    /// </summary>
    class ExtactionSubCategory : AbstractCategory
    {
        protected override void Add(string name)
        {
            Children.Add(name, new ExtactionItem() { Name = name });
        }
    }
}
