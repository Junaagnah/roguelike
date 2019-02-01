using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class Cbdd
{
    private MySqlConnection connection;
    private int score;

    public Cbdd()
    {
        this.InitConnection();
    }

    private void InitConnection()
    {
        string connectionString = "SERVER=185.213.24.116;DATABASE=roguelike;PORT=3306;UID=roguelike;PASSWORD=cciroguelike2018";
        this.connection = new MySqlConnection(connectionString);
    }

    //Récupère une liste contenant les difficultés dans la base de données
    public List<Difficulty> GetDifficulties()
    {
        List<Difficulty> diffs = new List<Difficulty>();

        try
        {
            this.connection.Open();

            MySqlCommand query = this.connection.CreateCommand();

            query.CommandText = "SELECT id_difficulte, nom_diff, level_length, dmg_player, dmg_mob, spawn_mob, minSpawn_wall, maxSpawn_wall, minFood_spawn, maxFood_spawn, coef_money FROM difficulte";

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
                        //reader[6] : minSpawn_wall
                        //reader[7] : maxSpawn_wall
                        //reader[8] : minFood_spawn
                        //reader[9] : maxFood_spawn
                        //reader[10] : coef_money

                        diffs.Add(new Difficulty(Convert.ToInt32(reader[0]), Convert.ToString(reader[1]), Convert.ToInt32(reader[2]), Convert.ToDecimal(reader[3]), Convert.ToDecimal(reader[4]), Convert.ToInt32(reader[5]), Convert.ToInt32(reader[6]), Convert.ToInt32(reader[7]), Convert.ToInt32(reader[8]), Convert.ToInt32(reader[9]), Convert.ToDecimal(reader[10])));
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

    //Enregistre le score du joueur dans la bdd
    public void SaveGame(Score score, Difficulty diff)
    {
        try
        {
            this.connection.Open();

            MySqlCommand query = this.connection.CreateCommand();

            query.CommandText = "INSERT INTO partie (date, niveau_perso, force_perso, argent_perso, tours, monstres, boss, FKid_user, FKid_difficulte) VALUES (current_timestamp(), @niveau, @force, @argent, @tours, @monstres, @boss, @iduser, @iddiff)";

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

    //Récupère la liste des scores en fonction de la difficultée entrée en paramètre
    public List<Score> GetScores(int diff)
    {
        List<Score> scores = new List<Score>();

        try
        {
            this.connection.Open();

            MySqlCommand query = this.connection.CreateCommand();

            query.CommandText = "SELECT u.username, (p.niveau_perso + p.force_perso + p.argent_perso + p.tours + p.monstres + p.boss) as scoreUser FROM partie as p LEFT JOIN users as u ON p.FKid_user = u.id LEFT JOIN difficulte as d ON p.FKid_difficulte = d.id_difficulte WHERE p.FKid_difficulte = @idDiff ORDER BY scoreUser DESC LIMIT 10";

            query.Parameters.AddWithValue("@idDiff", diff);

            using (MySqlDataReader reader = query.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        //reader[0] : username
                        //reader[1] : score
                        score = Convert.ToInt32(reader[1]);

                        switch(diff)
                        {
                            case 2:
                                score *= 2;
                                break;
                            case 3:
                                score *= 3;
                                break;
                        }

                        scores.Add(new Score(Convert.ToString(reader[0]), score));
                    }
                }
            }

            this.connection.Close();
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }

        return scores;
    }
}
