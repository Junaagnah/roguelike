﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class Cbdd
{
    private MySqlConnection connection;

    public Cbdd()
    {
        this.InitConnection();
    }

    private void InitConnection()
    {
        string connectionString = "SERVER=185.213.24.116;DATABASE=roguelike;PORT=3306;UID=roguelike;PASSWORD=cciroguelike2018";
        this.connection = new MySqlConnection(connectionString);
    }

    public List<Difficulty> GetDifficulties()
    {
        List<Difficulty> diffs = new List<Difficulty>();

        try
        {
            this.connection.Open();

            MySqlCommand query = this.connection.CreateCommand();

            query.CommandText = "SELECT id_difficulte, nom_diff, nb_salles, dmg_player, dmg_mob, spawn_mob, spawn_potions FROM difficulte";

            using (MySqlDataReader reader = query.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //reader[0] : Id
                        //reader[1] : NomDiff
                        //reader[2] : NbSalles
                        //reader[3] : DmgPlayer
                        //reader[4] : DmgMob
                        //reader[5] : SpawnMob
                        //reader[6] : SpawnPotions

                        diffs.Add(new Difficulty(Convert.ToInt32(reader[0]), Convert.ToString(reader[1]), Convert.ToInt32(reader[2]), Convert.ToInt32(reader[3]), Convert.ToInt32(reader[4]), Convert.ToInt32(reader[5]), Convert.ToInt32(reader[6])));
                    }
                }
            }

            this.connection.Close();
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }

        return diffs;
    }

    public void SaveGame(Score score, Difficulty diff)
    {
        try
        {
            this.connection.Open();

            MySqlCommand query = this.connection.CreateCommand();

            query.CommandText = "INSERT INTO partie (date, niveau_perso, force_perso, argent_perso, tours, monstres, boss, #id_user, #id_difficulte) VALUES (NOW(), @niveau, @force, @argent, @tours, @monstres, @boss, @iduser, @iddiff)";

            query.Parameters.AddWithValue("@niveau", score.NiveauPerso);
            query.Parameters.AddWithValue("@force", score.ForcePerso);
            query.Parameters.AddWithValue("@argent", score.ArgentPerso);
            query.Parameters.AddWithValue("@tours", score.Tours);
            query.Parameters.AddWithValue("@monstres", score.Monstres);
            query.Parameters.AddWithValue("@boss", score.Boss);
            query.Parameters.AddWithValue("@iduser", score.IdUser);
            query.Parameters.AddWithValue("@iddiff", diff.Id);

            query.ExecuteNonQuery();

            this.connection.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}