using System.Collections.Generic;
using System.Windows.Controls;

namespace StoreManagement
{
    class SuggestionsManager
    {
        ComboBox Account;
        ComboBox Category;
        ComboBox Sub_Category;
        ComboBox Detail;

        public SuggestionsManager(ComboBox Account, ComboBox Category, ComboBox Sub_Category, ComboBox Detail)
        {
            this.Account = Account;
            this.Category = Category;
            this.Sub_Category = Sub_Category;
            this.Detail = Detail;

            Account.Items.Add("M1");
            Account.Items.Add("M3");
            Account.Items.Add("M4");
            Account.Items.Add("M5");
            Account.Items.Add("M6");
            Account.Items.Add("M7");
            Account.Items.Add("M8");
            Account.Items.Add("EMROIDERY");
            Account.Items.Add("STORE");
            Account.Items.Add("DISPOSE");
            Account.Items.Add("STITCHING");

            Category.Items.Add("THREAD");
            Category.Items.Add("ACCESSORY");
            Category.Items.Add("SEQUIN");
            Category.Items.Add("DESIGNS");
            Category.Items.Add("FABRIC");

            (Category.Template.FindName("PART_EditableTextBox", Category) as TextBox).TextChanged += Category_TextChanged;
            (Sub_Category.Template.FindName("PART_EditableTextBox", Sub_Category) as TextBox).TextChanged += Sub_Category_TextChanged;
        }

        public void Category_TextChanged(object sender, TextChangedEventArgs e)
        {
            Sub_Category.Items.Clear();

            if ((sender as TextBox).Text == "THREAD")
            {
                Sub_Category.Items.Add("BOBIN");

                Sub_Category.Items.Add("TILLA-NEW");
                Sub_Category.Items.Add("TILLA-USED");

                Sub_Category.Items.Add("VISCOSE-NEW");
                Sub_Category.Items.Add("VISCOSE-USED");

                Sub_Category.Items.Add("POLYSTER-NEW");
                Sub_Category.Items.Add("POLYSTER-USED");
            }
            else if ((sender as TextBox).Text == "ACCESSORY")
            {
                Sub_Category.Items.Add("NEEDLE");
                Sub_Category.Items.Add("KAROSHIA");
                Sub_Category.Items.Add("FUSING");
            }
            else if ((sender as TextBox).Text == "SEQUIN")
            {
                Sub_Category.Items.Add("SEQUIN-NEW");
                Sub_Category.Items.Add("SEQUIN-USED");

                Sub_Category.Items.Add("SEQUIN-ROLL");

                Sub_Category.Items.Add("DISK");
            }
            else if ((sender as TextBox).Text == "DESIGNS")
            {
                Sub_Category.Items.Add("BW103");
                Sub_Category.Items.Add("BW124");
            }
            else if ((sender as TextBox).Text == "FABRIC")
            {
                Sub_Category.Items.Add("SHAFOON");
                Sub_Category.Items.Add("TISSUE");
                Sub_Category.Items.Add("LAWN");
            }
        }

        public void Sub_Category_TextChanged(object sender, TextChangedEventArgs e)
        {
            Detail.Items.Clear();

            if (Sub_Category.Text.StartsWith("VISCOSE")
                || Sub_Category.Text.StartsWith("POLYSTER")
                || Category.Text == "FABRIC")
            {
                Detail.Items.Add("1118");
                Detail.Items.Add("1278");
                Detail.Items.Add("1147");
                Detail.Items.Add("1380");
                Detail.Items.Add("4192");
                Detail.Items.Add("1001");
                Detail.Items.Add("1000");
            }
            else if (Sub_Category.Text.StartsWith("BOBIN"))
            {
                Detail.Items.Add("SKIN");
                Detail.Items.Add("BLACK");
                Detail.Items.Add("WHITE");
                Detail.Items.Add("TAMBAKOO");
                Detail.Items.Add("FEROZEE");
                Detail.Items.Add("GREEN");
            }
            else if (Sub_Category.Text.StartsWith("TILLA"))
            {
                Detail.Items.Add("1552");
                Detail.Items.Add("7272");
                Detail.Items.Add("TAMBAKOO");
            }
            else if (Sub_Category.Text.StartsWith("NEEDLE"))
            {
                Detail.Items.Add("FLAT-14");
                Detail.Items.Add("ARI");
            }
            else if (Sub_Category.Text.StartsWith("SEQUIN"))
            {
                Detail.Items.Add("SILVER GLITTER");
                Detail.Items.Add("TAMBAKOO GLITTER");
                Detail.Items.Add("GOLDEN GLITTER");
                Detail.Items.Add("GOLDEN PLAIN");
                Detail.Items.Add("SILVER");
            }
            else if (Sub_Category.Text.StartsWith("DISK"))
                Detail.Items.Add("EMPTY");
            else if (Sub_Category.Text.StartsWith("BW"))
            {
                Detail.Items.Add("KURTI");
                Detail.Items.Add("TROUSER");
                Detail.Items.Add("DUPATTA");
            }
        }
    }
}