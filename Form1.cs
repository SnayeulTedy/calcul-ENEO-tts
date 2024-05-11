using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace ENEO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Methodes de calcul de facture

        private double CalculerFactureBT(int consommation)
        {
            if (consommation <= 110)
                return consommation * 50;
            else if (consommation <= 400)
                return (110 * 50) + ((consommation - 110) * 79);
            else if (consommation <= 800)
                return (110 * 50) + (290 * 79) + ((consommation - 400) * 94);
            else
                return (110 * 50) + (290 * 79) + (400 * 94) + ((consommation - 800) * 99);
        }

        private double CalculerFactureMT(int consommation)
        {
            double primeFixe = 3700;
            double tarifparkwh = 150;
            double total = primeFixe + (tarifparkwh * consommation);
            return total;
        }
        #endregion


        #region Evenements et Controles

        private void btnCalculer_Click(object sender, EventArgs e)
        {
            string nomClient = txtNom.Text;
            string prenomClient = txtPrenom.Text;
            string villeClient = txtVille.Text;
            string quartierClient = txtQuartier.Text;
            string identifiantClient = txtIdentifiant.Text;
            int consommation;
            double montantTotal;

            if(int.TryParse(txtConsommation.Text, out consommation) && consommation >= 0)
            {
                if(rbBT.Checked)
                {
                    montantTotal = CalculerFactureBT(consommation);
                }
                else if(rbMT.Checked)
                {
                    montantTotal = CalculerFactureMT(consommation);
                }
                else
                {
                    MessageBox.Show("Veuillez selectionner une categorie de client.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string dateEmission = DateTime.Now.ToString("dd-mm-yyyy");
                string contenuFacture = "Facture d'electricité pour " + prenomClient + " " + nomClient + " (ID: " + identifiantClient + ") émis le " + dateEmission + ":\r\n" +
                                        "Ville : " + villeClient + "\r\n" +
                                        "Quartier : " + quartierClient + "\r\n" +
                                        "Consommation : " + consommation + "kwh\r\n" +
                                        "Montent Total : " + montantTotal + "FCFA\r\n";
                rtbFacture.Text = contenuFacture;
            }
            else
            {
                MessageBox.Show("Veuillez saisir une consommation valide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnModifier_Click(object sender, EventArgs e)
        {
            ActiverModification();
        }
     
        private void btnCharger_Click(object sender, EventArgs e)
        {
            ChargerFacture();
        }
       
        private void btnEnregistrer_Click(object sender, EventArgs e)
        {
            EnregistrerFacture();
        }

        #endregion

        #region Methodes Auxiliaires

        private void ActiverModification()
        {
            txtNom.ReadOnly = false;
            txtPrenom.ReadOnly = false;
            txtVille.ReadOnly = false;
            txtQuartier.ReadOnly = false;
            txtIdentifiant.ReadOnly = false;
            txtConsommation.ReadOnly = false;
            rbBT.Enabled = true;
            rbMT.Enabled = true;
            rtbFacture.ReadOnly = false;

            MessageBox.Show("Vous pouvez maintenant modifier la facture.", "Modification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EnregistrerFacture()
        {
            txtNom.ReadOnly = true;
            txtPrenom.ReadOnly = true;
            txtVille.ReadOnly = true;
            txtQuartier.ReadOnly = true;
            txtIdentifiant.ReadOnly = true;
            txtConsommation.ReadOnly = true;
            rbBT.Enabled = false;
            rbMT.Enabled = false;
            rtbFacture.ReadOnly = true;

            string dateEmission = DateTime.Now.ToString("dd-mm-yyyy");
            string nomFichier = "Facture_" + txtIdentifiant.Text + "_" + dateEmission;
            int numSequence = 1;
            string cheminFichier = "";

            do
            {
                cheminFichier = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nomFichier + "_" + numSequence + ".txt");
                if (!File.Exists(cheminFichier))
                {
                    break;
                }
                numSequence++;
            } while (true);
            string contenuFacture = rtbFacture.Text;
            File.WriteAllText(cheminFichier, contenuFacture);

            MessageBox.Show("La facture a été enregistrée dans : " + cheminFichier, "Facture enregistrée.", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ChargerFacture()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fichiers texte (*.txt)|*.txt|Tous les fichiers(*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string contenuFacture = File.ReadAllText(openFileDialog.FileName);
                    rtbFacture.Text = contenuFacture;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Une erreur s'est produite lors du chargement du fichier : " + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion
    }
}