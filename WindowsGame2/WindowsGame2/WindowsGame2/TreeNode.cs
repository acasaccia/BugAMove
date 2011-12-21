using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame2
{
    class TreeNode<TYPE>
    {
        private TYPE data;
        private IList<TreeNode<TYPE>> children;
        private TreeNode<TYPE> father;
        public TreeNode(TYPE data){
            this.data = data;
            this.children = new List<TreeNode<TYPE>>();
        }

        public void setFather(TreeNode<TYPE> father) {
            this.father = father;
        }
        public TreeNode<TYPE> getFather() {
            return this.father;
        }
        public void addChild(TreeNode<TYPE> c){
            this.children.Add(c);
            c.setFather(this);
        }
        public void removeChild(TreeNode<TYPE> c) {
            this.children.Remove(c);
            c.setFather(null);
        }
        public TreeNode<TYPE> removeChild(int index) {
            TreeNode<TYPE> n= this.children[index];
            this.children.Remove(n);
            n.setFather(null);
            return n;
        }
    }
}
