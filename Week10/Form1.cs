using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Week10
{
    public partial class FormHasiPertandingan : Form
    {
        public FormHasiPertandingan()
        {
            InitializeComponent();
        }
        static string connectionString = "server=localhost;uid=root;pwd=;database=premier_league;";
        public MySqlConnection sqlConnect = new MySqlConnection(connectionString);
        public MySqlCommand sqlCommand;
        public MySqlDataAdapter sqlAdapter;
        public string sqlQuery;

        DataTable dtTeamAway = new DataTable();
        DataTable dtTeamHome = new DataTable();

        private void FormHasiPertandingan_Load(object sender, EventArgs e)
        {
            sqlQuery = "SELECT t.team_id as `ID Tim`, t.team_name as `Nama Tim`, m.manager_name as `Nama Manager`, IF(m2.manager_name IS NULL,'----',m2.manager_name) as `Nama Asisten Manager`,p.player_name as `Nama Kapten`, home_stadium as `Stadium`, capacity as `Kapasitas` FROM team t LEFT JOIN manager m2 ON  t.assmanager_id = m2.manager_id ,manager m, player p WHERE t.manager_id = m.manager_id AND t.captain_id = p.player_id ;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtTeamHome);
            sqlAdapter.Fill(dtTeamAway);
            comboHome.DataSource = dtTeamHome;
            comboHome.DisplayMember = "Nama Tim";
            comboHome.ValueMember = "ID Tim";
            comboAway.DataSource = dtTeamAway;
            comboAway.DisplayMember = "Nama Tim";
        }

        private void comboHome_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posisiIndex = comboHome.SelectedIndex;
            lblHomeManager.Text = dtTeamHome.Rows[posisiIndex]["Nama Manager"].ToString();
            lblHomeCaptain.Text = dtTeamHome.Rows[posisiIndex]["Nama Kapten"].ToString();
            lblStadiumName.Text = dtTeamHome.Rows[posisiIndex]["Stadium"].ToString();
            lblStadiumCapacity.Text = dtTeamHome.Rows[posisiIndex]["Kapasitas"].ToString();
        }

        private void comboAway_SelectedIndexChanged(object sender, EventArgs e)
        {
            int posisiIndex = comboAway.SelectedIndex;
            lblAwayManager.Text = dtTeamAway.Rows[posisiIndex]["Nama Manager"].ToString();
            lblAwayCaptain.Text = dtTeamAway.Rows[posisiIndex]["Nama Kapten"].ToString();
        }

        private void btnchck_Click(object sender, EventArgs e)
        {
            DataTable tanggalSkor = new DataTable();
            sqlQuery = "select date_format(m.match_date, \"%e %M %Y\") as Tanggal, concat(m.goal_home, ' - ', m.goal_away) as Skor from `match` m " +
                "where m.team_home = '" + comboHome.SelectedValue.ToString() + "' and m.team_away = '" + comboAway.SelectedValue.ToString() + "'";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(tanggalSkor);
            lblDate.Text = tanggalSkor.Rows[0]["Tanggal"].ToString();
            lblScore.Text = tanggalSkor.Rows[0]["Skor"].ToString();
            DataTable detailMatch = new DataTable();
            sqlQuery = "select dm1.minute as Minute, if(p.team_id != m.team_home, '', p.player_name) as 'Player Name 1', " +
                "if(p.team_id != m.team_home, '', if(dm1.type = 'CY', 'Yellow Card', if(dm1.type = 'CR', 'Red Card', if(dm1.type = 'GO', 'Goal', if(dm1.type = 'GP', 'Goal Penalty', " +
                "if(dm1.type = 'GW', 'Own Goal', if(dm1.type = 'PM', 'Penalty Miss', ''))))))) as 'Tipe 1', if(p.team_id != m.team_away, '', p.player_name) as 'Player Name 2', " +
                "if(p.team_id != m.team_away, '', if(dm1.type = 'CY', 'Yellow Card', if(dm1.type = 'CR', 'Red Card', if(dm1.type = 'GO', 'Goal', if(dm1.type = 'GP', 'Goal Penalty', if(dm1.type = 'GW', 'Own Goal', " +
                "if(dm1.type = 'PM', 'Penalty Miss', ''))))))) as 'Tipe 2'  from dmatch dm1, player p, `match` m where dm1.match_id = m.match_id and p.player_id = dm1.player_id and m.team_home = '" + comboHome.SelectedValue.ToString() + "' and m.team_away = '" + comboAway.SelectedValue.ToString() + "' order by 1";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(detailMatch);
            dgvData.DataSource = detailMatch;
        }
    }
}
